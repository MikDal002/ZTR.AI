﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MathNet.Numerics.LinearAlgebra.Double;
using NUnit.Framework;
using ZTR.AI.Algorithms.Core;
using ZTR.AI.Common.Core.RandomEngines;

namespace ZTR.AI.Common.Core.Tests;

public class CollectionAssertionsExtensionsTests
{
    [Test]
    public void VectorMustBeTheSameCountAs_ThrowArgumentException_WhenCountDiffers()
    {
        var vector1 = Vector.Build.Dense(1, 1);
        var vector2 = Vector.Build.Dense(2, 1);

        var action = () => vector1.MustBeTheSameCountAs(vector2);
        action.Should().Throw<ArgumentException>();
    }

    [Test]
    public void VectorMustBeTheSameCountAs_ReturnsFirstParameter_WhenCountIsTheSame()
    {
        var vector1 = Vector.Build.Dense(1, 1);
        var vector2 = Vector.Build.Dense(1, 2);

        var result = vector1.MustBeTheSameCountAs(vector2);
        result.Should().BeSameAs(vector1);
    }
}

public class NextDoubleFromRangeTests
{
    private SystemRandomEngine _random = null!;

    [SetUp]
    public void SetUp()
    {
        // Arrange
        _random = new SystemRandomEngine();
    }
    
    [Test]
    public void MinValue_MustBeLower_ThanMaxValue()
    {
        // Act
        var action = () => _random.NextDoubleFromRange(5, 0);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [TestCase(double.PositiveInfinity, double.PositiveInfinity)]
    [TestCase(double.NegativeInfinity, double.NegativeInfinity)]
    [TestCase(double.NaN, 0.0)]
    [TestCase(0.0, double.NaN)]
    public void MinValue_AndMaxValue_CannotBeTheSameInfinite(double min, double max)
    {
        // Act
        var action = () => _random.NextDoubleFromRange(min, max);
        
        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Test]
    public void GivenMinValueAndMaxValueTheSame_ReturnsTheSame([Range(-10.0, 10.0, 1.0)] double value)
    {
        var result = _random.NextDoubleFromRange(value, value);

        result.Should().Be(value);
    }
}