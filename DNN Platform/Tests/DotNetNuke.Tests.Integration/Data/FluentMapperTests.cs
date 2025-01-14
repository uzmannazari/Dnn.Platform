﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Tests.Data
{
    using System;

    using DotNetNuke.Data.PetaPoco;
    using DotNetNuke.Tests.Data.Models;
    using DotNetNuke.Tests.Utilities;
    using NUnit.Framework;
    using PetaPoco;

    [TestFixture]
    public class FluentMapperTests
    {
        [Test]
        public void FluentMapper_Constructor_Initialises_TablePrefix_Property()
        {
            // Arrange

            // Act
            var mapper = new FluentMapper<Dog>(Constants.TABLENAME_Prefix);

            // Assert
            Assert.That(Util.GetPrivateMember<FluentMapper<Dog>, string>(mapper, "_tablePrefix"), Is.EqualTo(Constants.TABLENAME_Prefix));
        }

        [Test]
        public void FluentMapper_GetTableInfo_Returns_TableInfo()
        {
            // Arrange
            var mapper = new FluentMapper<Dog>(Constants.TABLENAME_Prefix);

            // Act
            var ti = mapper.GetTableInfo(typeof(Dog));

            // Assert
            Assert.That(ti, Is.InstanceOf<TableInfo>());
        }

        [Test]
        public void FluentMapper_Maps_TableName_To_Value_Provided()
        {
            // Arrange
            var mapper = new FluentMapper<Dog>(string.Empty);
            mapper.TableName(Constants.TABLENAME_Dog);

            // Act
            var ti = mapper.GetTableInfo(typeof(Dog));

            // Assert
            Assert.That(ti.TableName, Is.EqualTo(Constants.TABLENAME_Dog));
        }

        [Test]
        public void FluentMapper_Adds_TablePrefix_To_Value_Provided()
        {
            // Arrange
            var mapper = new FluentMapper<Dog>(Constants.TABLENAME_Prefix);
            mapper.TableName(Constants.TABLENAME_Dog);

            // Act
            var ti = mapper.GetTableInfo(typeof(Dog));

            // Assert
            Assert.That(ti.TableName, Is.EqualTo(Constants.TABLENAME_Prefix + Constants.TABLENAME_Dog));
        }

        [Test]
        public void FluentMapper_Does_Not_Map_TableName_To_Attribute()
        {
            // Arrange
            var mapper = new FluentMapper<Person>(string.Empty);
            mapper.TableName(Constants.TABLENAME_Dog);

            // Act
            var ti = mapper.GetTableInfo(typeof(Person));

            // Assert
            Assert.That(ti.TableName, Is.Not.EqualTo(Constants.TABLENAME_Person));
            Assert.That(ti.TableName, Is.EqualTo(Constants.TABLENAME_Dog));
        }

        [Test]
        public void FluentMapper_Sets_TableName_To_Null_If_Not_Provided()
        {
            // Arrange
            var mapper = new FluentMapper<Person>(string.Empty);

            // Act
            var ti = mapper.GetTableInfo(typeof(Person));

            // Assert
            Assert.That(ti.TableName, Is.Null);
        }

        [Test]
        public void FluentMapper_Sets_PrimaryKey_To_Provided_Value()
        {
            // Arrange
            var mapper = new FluentMapper<Person>(string.Empty);
            mapper.PrimaryKey(Constants.TABLENAME_Key);

            // Act
            var ti = mapper.GetTableInfo(typeof(Person));

            // Assert
            Assert.That(ti.PrimaryKey, Is.EqualTo(Constants.TABLENAME_Key));
        }

        [Test]
        public void FluentMapper_Does_Not_Set_PrimaryKey_To_Attribute()
        {
            // Arrange
            var mapper = new FluentMapper<Person>(string.Empty);
            mapper.PrimaryKey(Constants.TABLENAME_Key);

            // Act
            var ti = mapper.GetTableInfo(typeof(Person));

            // Assert
            Assert.That(ti.PrimaryKey, Is.Not.EqualTo(Constants.TABLENAME_Person_Key));
        }

        [Test]
        public void FluentMapper_Sets_PrimaryKey_To_Null_If_Not_Provided()
        {
            // Arrange
            var mapper = new FluentMapper<Person>(string.Empty);

            // Act
            var ti = mapper.GetTableInfo(typeof(Person));

            // Assert
            Assert.That(ti.PrimaryKey, Is.Null);
        }

        [Test]
        public void FluentMapper_Sets_AutoIncrement_To_False()
        {
            // Arrange
            var mapper = new FluentMapper<Person>(string.Empty);

            // Act
            var ti = mapper.GetTableInfo(typeof(Person));

            // Assert
            Assert.That(ti.AutoIncrement, Is.False);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void FluentMapper_Sets_AutoIncrement_To_Value_Provided(bool autoIncrement)
        {
            // Arrange
            var mapper = new FluentMapper<Person>(string.Empty);
            mapper.PrimaryKey("ID", autoIncrement);

            // Act
            var ti = mapper.GetTableInfo(typeof(Person));

            // Assert
            Assert.That(ti.AutoIncrement, Is.EqualTo(autoIncrement));
        }

        [Test]
        public void FluentMapper_GetColumnInfo_Returns_Null_If_Not_Provided()
        {
            // Arrange
            var mapper = new FluentMapper<Dog>(Constants.TABLENAME_Prefix);

            var dogType = typeof(Dog);
            var dogProperty = dogType.GetProperty(Constants.COLUMNNAME_Name);

            // Act
            var ci = mapper.GetColumnInfo(dogProperty);

            // Assert
            Assert.That(ci, Is.Null);
        }

        [Test]
        public void FluentMapper_GetColumnInfo_Returns_ColumnInfo()
        {
            // Arrange
            var mapper = new FluentMapper<Dog>(Constants.TABLENAME_Prefix);
            mapper.Property(d => d.Name, Constants.COLUMNNAME_Name);

            var dogType = typeof(Dog);
            var dogProperty = dogType.GetProperty(Constants.COLUMNNAME_Name);

            // Act
            var ci = mapper.GetColumnInfo(dogProperty);

            // Assert
            Assert.That(ci, Is.InstanceOf<ColumnInfo>());
        }

        [Test]
        public void FluentMapper_GetColumnInfo_Maps_ColumnName_To_Value_Provided()
        {
            // Arrange
            var mapper = new FluentMapper<Dog>(Constants.TABLENAME_Prefix);
            mapper.Property(d => d.Name, Constants.COLUMNNAME_Name);

            var personType = typeof(Dog);
            var personProperty = personType.GetProperty(Constants.COLUMNNAME_Name);

            // Act
            var ci = mapper.GetColumnInfo(personProperty);

            // Assert
            Assert.That(ci.ColumnName, Is.EqualTo(Constants.COLUMNNAME_Name));
        }

        [Test]
        public void FluentMapper_GetColumnInfo_Does_Not_Map_ColumnName_To_Attribute()
        {
            // Arrange
            var mapper = new FluentMapper<Person>(Constants.TABLENAME_Prefix);
            mapper.Property(d => d.Name, Constants.COLUMNNAME_Name);

            var personType = typeof(Person);
            var personProperty = personType.GetProperty(Constants.COLUMNNAME_Name);

            // Act
            var ci = mapper.GetColumnInfo(personProperty);

            // Assert
            Assert.That(ci.ColumnName, Is.Not.EqualTo(Constants.COLUMNNAME_PersonName));
        }
    }
}
