// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Ldap.Novell.Entries;

/// <summary>
/// Flags that control the behavior of user accounts in Active Directory (LDAP).
/// These values correspond to the userAccountControl attribute and are used as bitwise flags.
/// Some flags are deprecated and should not be used in new code.
/// </summary>
[Flags]
public enum UserAccountControl
{
    /// <summary>
    /// The logon script will be run.
    /// </summary>
    Script = 1,

    /// <summary>
    /// The user account is disabled.
    /// </summary>
    AccountDisabled = 2,

    /// <summary>
    /// The home directory is required.
    /// </summary>
    HomeDirectoryRequired = 8,

    /// <summary>
    /// The account is locked out. Deprecated: Use lockoutTime attribute instead.
    /// </summary>
    AccountLockedOut_DEPRECATED = 16,

    /// <summary>
    /// No password is required.
    /// </summary>
    PasswordNotRequired = 32,

    /// <summary>
    /// The user cannot change the password. Deprecated: Use ntSecurityDescriptor instead.
    /// </summary>
    PasswordCannotChange_DEPRECATED = 64,

    /// <summary>
    /// The user can use reversible encryption for the password.
    /// </summary>
    EncryptedTextPasswordAllowed = 128,

    /// <summary>
    /// This is a temporary duplicate account.
    /// </summary>
    TempDuplicateAccount = 256,

    /// <summary>
    /// This is a normal user account.
    /// </summary>
    NormalAccount = 512,

    /// <summary>
    /// This is a trust account for a domain.
    /// </summary>
    InterDomainTrustAccount = 2048,

    /// <summary>
    /// This is a computer account for a workstation.
    /// </summary>
    WorkstationTrustAccount = 4096,

    /// <summary>
    /// This is a computer account for a server.
    /// </summary>
    ServerTrustAccount = 8192,

    /// <summary>
    /// The password does not expire.
    /// </summary>
    PasswordDoesNotExpire = 65536,

    /// <summary>
    /// This is an MNS logon account.
    /// </summary>
    MnsLogonAccount = 131072,

    /// <summary>
    /// Smart card is required for logon.
    /// </summary>
    SmartCardRequired = 262144,

    /// <summary>
    /// The account is trusted for Kerberos delegation.
    /// </summary>
    TrustedForDelegation = 524288,

    /// <summary>
    /// The account is not trusted for delegation.
    /// </summary>
    AccountNotDelegated = 1048576,

    /// <summary>
    /// Use only DES encryption types for this account.
    /// </summary>
    UseDesKeyOnly = 2097152,

    /// <summary>
    /// Do not require Kerberos preauthentication.
    /// </summary>
    DontRequirePreauth = 4194304,

    /// <summary>
    /// The user's password has expired. Deprecated: Use pwdLastSet attribute instead.
    /// </summary>
    PasswordExpired_DEPRECATED = 8388608,

    /// <summary>
    /// The account is trusted to authenticate for delegation.
    /// </summary>
    TrustedToAuthenticateForDelegation = 16777216,

    /// <summary>
    /// This is a read-only domain controller account.
    /// </summary>
    PartialSecretsAccount = 67108864
}
