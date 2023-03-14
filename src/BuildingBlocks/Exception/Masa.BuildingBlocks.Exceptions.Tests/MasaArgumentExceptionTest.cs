// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Exceptions.Tests;

[TestClass]
public class MasaArgumentExceptionTest
{
    [TestMethod]
    public void TestThrowIfNull()
    {
        object? str = null;

        try
        {
            MasaValidatorException.ThrowIfNull("");

            MasaArgumentException.ThrowIfNull(str);
        }
        catch (MasaArgumentException ex)
        {
            Assert.AreEqual(Data.Constants.ExceptionErrorCode.NOT_NULL_VALIDATOR, ex.ErrorCode);
            Assert.AreEqual("'{0}' must not be empty.", ex.GetErrorMessage());
        }
    }

    [DataTestMethod]
    [DataRow(null, Data.Constants.ExceptionErrorCode.NOT_NULL_AND_EMPTY_VALIDATOR)]
    [DataRow("", Data.Constants.ExceptionErrorCode.NOT_NULL_AND_EMPTY_VALIDATOR)]
    public void TestThrowIfNullOrEmpty(string? value, string code)
    {
        MasaValidatorException.ThrowIfNullOrEmpty("test");
        try
        {
            MasaArgumentException.ThrowIfNullOrEmpty(value);
        }
        catch (MasaArgumentException ex)
        {
            Assert.AreEqual(code, ex.ErrorCode);
        }
    }

    [DataTestMethod]
    [DataRow(null, Data.Constants.ExceptionErrorCode.NOT_NULL_AND_WHITESPACE_VALIDATOR)]
    [DataRow("", Data.Constants.ExceptionErrorCode.NOT_NULL_AND_WHITESPACE_VALIDATOR)]
    [DataRow(" ", Data.Constants.ExceptionErrorCode.NOT_NULL_AND_WHITESPACE_VALIDATOR)]
    public void TestThrowIfNullOrWhiteSpace(string? value, string code)
    {
        MasaValidatorException.ThrowIfNullOrWhiteSpace("test");
        try
        {
            MasaArgumentException.ThrowIfNullOrWhiteSpace(value);
        }
        catch (MasaArgumentException ex)
        {
            Assert.AreEqual(code, ex.ErrorCode);
        }
    }

    [TestMethod]
    public void TestThrowIfNullOrEmptyCollection()
    {
        List<string>? list = null;
        try
        {
            MasaArgumentException.ThrowIfNullOrEmptyCollection(list);
        }
        catch (MasaArgumentException ex)
        {
            Assert.AreEqual(Data.Constants.ExceptionErrorCode.NOT_NULL_AND_EMPTY_COLLECTION_VALIDATOR, ex.ErrorCode);
        }
    }
}
