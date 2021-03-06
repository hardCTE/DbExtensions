﻿using System;
using System.Data.Common;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DbExtensions.Tests.Mapping {
   
   [TestClass]
   public class PocoMappingBehavior {

      readonly DbConnection conn = System.Data.SqlClient.SqlClientFactory.Instance
         .CreateSqlServerConnectionForTests();

      [TestMethod]
      public void Map_Property() {

         var value = conn.CreateCommand(SQL
            .SELECT("'a' AS Foo"))
            .Map<PocoMapping.Map_Property>()
            .Single();

         Assert.AreEqual("a", value.Foo);
      }

      [TestMethod]
      public void Map_Null_Property() {

         var value = conn.CreateCommand(SQL
            .SELECT("NULL AS Foo"))
            .Map<PocoMapping.Map_Null_Property>()
            .Single();

         Assert.IsNull(value.Foo);
      }

      [TestMethod]
      public void Map_Property_Private_Setter() {

         var value = conn.CreateCommand(SQL
            .SELECT("'a' AS Foo"))
            .Map<PocoMapping.Map_Property_Private_Setter>()
            .Single();

         Assert.AreEqual("a", value.Foo);
      }

      [TestMethod]
      public void Ignore_Unmapped_Property() {

         var value = conn.CreateCommand(SQL
            .SELECT("'a' AS Foo, 'b' AS Bar"))
            .Map<PocoMapping.Ignore_Unmapped_Property>()
            .Single();
      }

      [TestMethod]
      public void Map_Complex_Property() {

         var value = conn.CreateCommand(SQL
            .SELECT("'b' AS Bar$Foo"))
            .Map<PocoMapping.Map_Complex_Property>()
            .Single();

         Assert.IsNotNull(value.Bar);
         Assert.AreEqual("b", value.Bar.Foo);
      }

      [TestMethod]
      public void Map_Null_Complex_Property() {

         var value = conn.CreateCommand(SQL
            .SELECT("NULL AS Bar"))
            .Map<PocoMapping.Map_Null_Complex_Property>()
            .Single();

         Assert.IsNull(value.Bar);
      }

      [TestMethod]
      public void Map_Complex_Property_To_Null_When_All_Subproperties_Are_Null() {

         var value = conn.CreateCommand(SQL
            .SELECT("NULL AS Nested$Foo")
            ._("NULL AS Nested$Bar"))
            .Map<PocoMapping.Map_Complex_Property_To_Null_When_All_Subproperties_Are_Null>()
            .Single();

         Assert.IsNull(value.Nested);
      }

      [TestMethod]
      public void Load_Complex_Property() {

         var value = conn.CreateCommand(SQL
            .SELECT("2 AS Foo$B"))
            .Map<PocoMapping.Load_Complex_Property>()
            .Single();

         Assert.AreEqual(1, value.Foo.A);
      }

      [TestMethod]
      public void Map_Constructor() {

         var value = conn.CreateCommand(SQL
            .SELECT("'http://example.com' AS '1'"))
            .Map<Uri>()
            .Single();

         Assert.AreEqual("http://example.com", value.OriginalString);
      }

      [TestMethod, ExpectedException(typeof(InvalidOperationException))]
      public void Fail_When_Multiple_Constructors_With_Same_Number_Of_Parameters() {

         var value = conn.CreateCommand(SQL
            .SELECT("'http://example.com' AS '1', 1 AS '2'"))
            .Map<Uri>()
            .Single();
      }

      [TestMethod]
      public void Map_Constructor_Complex_Property() {

         var value = conn.CreateCommand(SQL
            .SELECT("'http://example.com' AS Url$1"))
            .Map<PocoMapping.Map_Constructor_Complex_Property>()
            .Single();

         Assert.IsNotNull(value.Url);
         Assert.AreEqual("http://example.com", value.Url.OriginalString);
      }

      [TestMethod]
      public void Map_Constructor_Nullable_Complex_Property() {

         var value = conn.CreateCommand(SQL
            .SELECT("1 AS Foo1$1")
            ._("NULL AS Foo2"))
            .Map<PocoMapping.Map_Constructor_Nullable_Complex_Property>()
            .Single();

         Assert.AreEqual(1, value.Foo1.Value.A);
         Assert.IsNull(value.Foo2);
      }

      [TestMethod]
      public void Map_Constructor_Complex_Property_To_Null_When_All_Arguments_And_Subproperties_Are_Null() {

         var value = conn.CreateCommand(SQL
            .SELECT("NULL AS Foo$1")
            ._("NULL AS Foo$Foo"))
            .Map<PocoMapping.Map_Constructor_Complex_Property_To_Null_When_All_Arguments_And_Subproperties_Are_Null>()
            .Single();

         Assert.IsNull(value.Foo);
      }

      [TestMethod]
      public void Map_Constructor_Complex_Argument_To_Null_When_All_Arguments_And_Subproperties_Are_Null() {

         var value = conn.CreateCommand(SQL
            .SELECT("NULL AS '1$1'")
            ._("NULL AS '1$Foo'"))
            .Map<PocoMapping.Map_Constructor_Complex_Argument_To_Null_When_All_Arguments_And_Subproperties_Are_Null>()
            .Single();

         Assert.IsNull(value.Foo);
      }

      [TestMethod]
      public void Load_Constructor_Complex_Property() {

         var value = conn.CreateCommand(SQL
            .SELECT("1 AS '1'")
            ._("2 AS Foo$A")
            ._("2 AS Foo$Bar$B"))
            .Map<PocoMapping.Load_Constructor_Complex_Property>()
            .Single();

         Assert.AreEqual(1, value.Foo.Bar.A);
      }

      [TestMethod]
      public void Load_Constructor_Complex_Argument() {

         var value = conn.CreateCommand(SQL
            .SELECT("2 AS '1$A'")
            ._("2 AS '1$Bar$B'"))
            .Map<PocoMapping.Load_Constructor_Complex_Argument>()
            .Single();

         Assert.AreEqual(1, value.Foo.Bar.A);
      }

      [TestMethod]
      public void Map_Null_Constructor_Argument() {

         var value = conn.CreateCommand(SQL
            .SELECT("1 AS '1'")
            ._("NULL AS '2'"))
            .Map<PocoMapping.Map_Null_Constructor_Argument>()
            .Single();

         Assert.IsNull(value.Url);
      }

      [TestMethod]
      public void Map_Constructor_Nested() {

         var value = conn.CreateCommand(SQL
            .SELECT("'http://example.com' AS '1$1'"))
            .Map<PocoMapping.Map_Constructor_Nested>()
            .Single();

         Assert.AreEqual("http://example.com", value.Url.OriginalString);
      }
   }
}

namespace DbExtensions.Tests.Mapping.PocoMapping {

   class Map_Property {
      public string Foo { get; set; }
   }

   class Map_Null_Property {
      public string Foo { get; set; }
   }

   class Map_Property_Private_Setter {
      public string Foo { get; private set; }
   }

   class Ignore_Unmapped_Property {
      public string Foo { get; set; }
   }

   class Map_Complex_Property {

      public string Foo { get; set; }
      public Map_Complex_Property Bar { get; set; }
   }

   class Map_Null_Complex_Property {

      public string Foo { get; set; }
      public Map_Null_Complex_Property Bar { get; set; }
   }

   class Map_Complex_Property_To_Null_When_All_Subproperties_Are_Null {

      public string Foo { get; set; }
      public string Bar { get; set; }
      public Map_Complex_Property_To_Null_When_All_Subproperties_Are_Null Nested { get; set; }
   }

   class Load_Complex_Property {

      public FooClass Foo { get; set; }

      public Load_Complex_Property() {
         this.Foo = new FooClass { 
            A = 1
         };
      }

      public class FooClass {
         public int A { get; set; }
         public int B { get; set; }
      }
   }

   class Map_Constructor_Complex_Property {
      public Uri Url { get; set; }
   }

   class Map_Constructor_Nullable_Complex_Property {

      public Foo? Foo1 { get; set; }
      public Foo? Foo2 { get; set; }
      
      public struct Foo {

         public readonly int A;

         public Foo(int a) {
            this.A = a;
         }
      }
   }

   class Map_Constructor_Complex_Property_To_Null_When_All_Arguments_And_Subproperties_Are_Null {

      public FooClass Foo { get; set; }

      public class FooClass {

         public string Foo { get; set; }

         public FooClass(int? id) { }
      }
   }

   class Map_Constructor_Complex_Argument_To_Null_When_All_Arguments_And_Subproperties_Are_Null {

      public readonly FooClass Foo;

      public Map_Constructor_Complex_Argument_To_Null_When_All_Arguments_And_Subproperties_Are_Null(FooClass foo) {
         this.Foo = foo;
      }

      public class FooClass {

         public string Foo { get; set; }

         public FooClass(int? id) { }
      }
   }

   class Load_Constructor_Complex_Property {

      public readonly int A;
      public FooClass Foo { get; set; }

      public Load_Constructor_Complex_Property(int a) {
         this.A = a;
      }

      public class FooClass {

         public int A { get; set; }
         public BarClass Bar { get; set; }

         public FooClass() {
            this.Bar = new BarClass { 
               A = 1
            };
         }

         public class BarClass {
            public int A { get; set; }
            public int B { get; set; }
         }
      }
   }

   class Load_Constructor_Complex_Argument {

      public readonly FooClass Foo;
      public int A { get; set; }

      public Load_Constructor_Complex_Argument(FooClass foo) {
         this.Foo = foo;
      }

      public class FooClass {

         public int A { get; set; }
         public BarClass Bar { get; set; }

         public FooClass() {
            this.Bar = new BarClass {
               A = 1
            };
         }

         public class BarClass {
            public int A { get; set; }
            public int B { get; set; }
         }
      }
   }

   class Map_Null_Constructor_Argument {

      public readonly int Id;
      public readonly Uri Url;

      public Map_Null_Constructor_Argument(int id) {
         this.Id = id;
      }

      public Map_Null_Constructor_Argument(int id, Uri url)
         : this(id) {

         this.Url = url;
      }
   }

   class Map_Constructor_Nested {

      public readonly Uri Url;

      public Map_Constructor_Nested(Uri url) {
         this.Url = url;
      }
   }
}
