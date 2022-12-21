// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.Expressions.Tests;

[TestClass]
public class ExpressionExtensionsTest
{
    private readonly List<int> _list;

    public ExpressionExtensionsTest()
    {
        _list = new List<int>()
        {
            1, 2, 3, 4, 5, 6, 7
        };
    }

    [TestMethod]
    public void TestAnd()
    {
        Expression<Func<int, bool>> condition = i => i > 0;
        condition = condition.And(i => i < 5);
        var list = _list.Where(condition.Compile()).ToList();
        Assert.AreEqual(4, list.Count);
        Assert.AreEqual(1, list[0]);
        Assert.AreEqual(2, list[1]);
        Assert.AreEqual(3, list[2]);
        Assert.AreEqual(4, list[3]);

        condition = i => i > 0;
        condition = condition.And(false, i => i < 5);

        list = _list.Where(condition.Compile()).ToList();
        Assert.AreEqual(7, list.Count);
        Assert.AreEqual(1, list[0]);
        Assert.AreEqual(2, list[1]);
        Assert.AreEqual(3, list[2]);
        Assert.AreEqual(4, list[3]);
        Assert.AreEqual(5, list[4]);
        Assert.AreEqual(6, list[5]);
        Assert.AreEqual(7, list[6]);
    }

    [TestMethod]
    public void TestOr()
    {
        Expression<Func<int, bool>> condition = i => i > 5;
        condition = condition.Or(i => i < 1);

        var list = _list.Where(condition.Compile()).ToList();
        Assert.AreEqual(2, list.Count);
        Assert.AreEqual(6, list[0]);
        Assert.AreEqual(7, list[1]);
    }

    [TestMethod]
    public void TestCompose()
    {
        Expression<Func<int, bool>> condition = i => i > 5;
        Expression<Func<int, bool>> condition2 = i => i < 7;
        var expression = condition.Compose(condition2, Expression.AndAlso);
        var list = _list.Where(expression.Compile()).ToList();
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual(6, list[0]);
    }
}
