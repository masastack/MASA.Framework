// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Development.DaprStarter;

/// <summary>
/// Allows processes to be automatically killed if this parent process unexpectedly quits.
/// This feature requires Windows 8 or greater. On Windows 7, nothing is done.</summary>
/// <remarks>References:
///  https://stackoverflow.com/a/4657392/386091
///  https://stackoverflow.com/a/9164742/386091 </remarks>
[ExcludeFromCodeCoverage]
public static class ChildProcessTracker
{
    /// <summary>
    /// Add the process to be tracked. If our current process is killed, the child processes
    /// that we are tracking will be automatically killed, too. If the child process terminates
    /// first, that's fine, too.</summary>
    /// <param name="process"></param>
    public static void AddProcess(Process? process)
    {
        if (process == null)
            return;

        if (SJobHandle != IntPtr.Zero)
        {
            bool success = AssignProcessToJobObject(SJobHandle, process.Handle);
            if (!success && !process.HasExited)
                throw new System.ComponentModel.Win32Exception();
        }
    }
#pragma warning disable S3963
#pragma warning disable S3877
    static ChildProcessTracker()
    {
        // This feature requires Windows 8 or later. To support Windows 7 requires
        //  registry settings to be added if you are using Visual Studio plus an
        //  app.manifest change.
        //  https://stackoverflow.com/a/4232259/386091
        //  https://stackoverflow.com/a/9507862/386091
        if (Environment.OSVersion.Version < new Version(6, 2))
            return;

        // The job name is optional (and can be null) but it helps with diagnostics.
        //  If it's not null, it has to be unique. Use SysInternals' Handle command-line
        //  utility: handle -a ChildProcessTracker
        string jobName = "ChildProcessTracker" + Environment.ProcessId;
        SJobHandle = CreateJobObject(IntPtr.Zero, jobName);

        var info = new JobObjectBasicLimitInformation
        {
            //  be killed, too.
            //  close the job handle, and when that happens, we want the child processes to
            // This is the key flag. When our process is killed, Windows will automatically
            LimitFlags = JobObjectLimits.JobObjectLimitKillOnJobClose
        };

        var extendedInfo = new JobObjectExtendedLimitInformation
        {
            BasicLimitInformation = info
        };

        int length = Marshal.SizeOf(typeof(JobObjectExtendedLimitInformation));
        IntPtr extendedInfoPtr = Marshal.AllocHGlobal(length);
        try
        {
            Marshal.StructureToPtr(extendedInfo, extendedInfoPtr, false);

            if (!SetInformationJobObject(SJobHandle, JobObjectInfoType.ExtendedLimitInformation,
                    extendedInfoPtr, (uint)length))
            {
                throw new System.ComponentModel.Win32Exception();
            }
        }
        finally
        {
            Marshal.FreeHGlobal(extendedInfoPtr);
        }
    }
#pragma warning restore S3877
#pragma warning restore S3963

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    static extern IntPtr CreateJobObject(IntPtr lpJobAttributes, string name);

    [DllImport("kernel32.dll")]
    static extern bool SetInformationJobObject(IntPtr job, JobObjectInfoType infoType, IntPtr lpJobObjectInfo, uint cbJobObjectInfoLength);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);

    // Windows will automatically close any open job handles when our process terminates.
    //  This can be verified by using SysInternals' Handle utility. When the job handle
    //  is closed, the child processes will be killed.
    private static readonly IntPtr SJobHandle;
}

public enum JobObjectInfoType
{
    AssociateCompletionPortInformation = 7,
    BasicLimitInformation = 2,
    BasicUiRestrictions = 4,
    EndOfJobTimeInformation = 6,
    ExtendedLimitInformation = 9,
    SecurityLimitInformation = 5,
    GroupInformation = 11
}

[StructLayout(LayoutKind.Sequential)]
public struct JobObjectBasicLimitInformation
{
    public Int64 PerProcessUserTimeLimit;
    public Int64 PerJobUserTimeLimit;
    public JobObjectLimits LimitFlags;
    public UIntPtr MinimumWorkingSetSize;
    public UIntPtr MaximumWorkingSetSize;
    public UInt32 ActiveProcessLimit;
    public Int64 Affinity;
    public UInt32 PriorityClass;
    public UInt32 SchedulingClass;
}

[Flags]
public enum JobObjectLimits : uint
{
    JobObjectLimitKillOnJobClose = 0x2000
}

[StructLayout(LayoutKind.Sequential)]
public struct IoCounters
{
    public UInt64 ReadOperationCount;
    public UInt64 WriteOperationCount;
    public UInt64 OtherOperationCount;
    public UInt64 ReadTransferCount;
    public UInt64 WriteTransferCount;
    public UInt64 OtherTransferCount;
}

[StructLayout(LayoutKind.Sequential)]
public struct JobObjectExtendedLimitInformation
{
    public JobObjectBasicLimitInformation BasicLimitInformation;
    public IoCounters IoInfo;
    public UIntPtr ProcessMemoryLimit;
    public UIntPtr JobMemoryLimit;
    public UIntPtr PeakProcessMemoryUsed;
    public UIntPtr PeakJobMemoryUsed;
}
