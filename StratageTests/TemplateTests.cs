using Microsoft.VisualStudio.TestTools.UnitTesting;
using Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Stratage;


namespace Template.Tests
{
    [TestClass()]
    public class TemplateTestscSHarp
    {
        [TestMethod()]
        public void TestIf()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new CSharp(), "{%?true%}true", null))
            {
                template.Render(output);
                Assert.AreEqual("true", output.ToString());
            }
        }

        [TestMethod()]
        public void TestIfWithArgumentTrue()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new CSharp(), "{%?a%}true", null, new Pairs("bool", "a")))
            {
                template.Render(output, true);
                Assert.AreEqual("true", output.ToString());
            }
        }
        [TestMethod()]
        public void TestIfWithArgumentsFalse()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new CSharp(), "{%?a%}true", null, new Pairs("bool", "a")))
            {
                template.Render(output, false);
                Assert.AreEqual("", output.ToString());
            }
        }
        [TestMethod()]
        public void TestLoop()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new CSharp(), "{%@ 2+2%}{%resultString+=i;%}", null))
            {
                template.Render(output);
                Assert.AreEqual("0123", output.ToString());
            }
        }
        [TestMethod()]
        public void TestLoopWithArguments()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new CSharp(), "{%@a%}{%=12%}", null, new Pairs("int", "a")))
            {
                template.Render(output, 10 + (-5));
                Assert.AreEqual("1212121212", output.ToString());
            }
        }
        [TestMethod()]
        public void TestCode()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new CSharp(), "{%if(true) resultString+=\"res\"; char[] ms={'t','r','o','o','m'}; foreach(var a in ms){resultString+=a;}%}", null))
            {
                template.Render(output);
                Assert.AreEqual("restroom", output.ToString());
            }
        }
        [TestMethod()]
        public void TestCodeWithParaetrs()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new CSharp(), "{%if(a) resultString+=\"res\"; char[] ms={'t','r','o','o','m'}; foreach(var c in ms){resultString+=c;}%}", null, new Pairs("bool", "a")))
            {
                template.Render(output, false);
                Assert.AreEqual("troom", output.ToString());
            }
        }

        [TestMethod()]
        public void TestExpressionn()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new CSharp(), "{%= (10==1)%}", null))
            {
                template.Render(output);
                Assert.AreEqual("False", output.ToString());
            }
        }
        [TestMethod()]
        public void TestExpressionnWithParametrs()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new CSharp(), "{%= (10==a)%}", null, new Pairs("int", "a")))
            {
                template.Render(output, 10);
                Assert.AreEqual("True", output.ToString());
            }
        }
        [TestMethod()]
        public void TestText()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new CSharp(), "I want to say Hello!", null))
            {
                template.Render(output);
                Assert.AreEqual("I want to say Hello!", output.ToString());
            }
        }
        [TestMethod()]
        public void TestEmptyString()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new CSharp(), "", null))
            {
                template.Render(output);
                Assert.AreEqual("", output.ToString());
            }
        }
        [TestMethod()]
        public void TestCompileError()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new CSharp(), "{% {%}", null))
            {
                template.Render(output);
                Assert.AreEqual("error", output.ToString());
            }
        }
        [TestMethod()]
        public void TestRuntieError()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new CSharp(), "{% char[] ms = { 'a', 'a' }; for(int i=0;i<3;i++){resultString+=ms[i];}%}", null))
            {
                template.Render(output);
                Assert.AreEqual("error", output.ToString());
            }
        }
        [TestMethod()]
        public void TestNotEnothArguments()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new CSharp(), "", null, new Pairs("int ", "a"), new Pairs("bool", "b"), new Pairs("char", "c"), new Pairs("string", "str")))
            {
                template.Render(output, 10, true, '\0');
                Assert.AreEqual("error", output.ToString());
            }
        }
        [TestMethod()]
        public void TestArguments()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new CSharp(), "", null, new Pairs("int ", "a"), new Pairs("bool", "b"), new Pairs("char", "c"), new Pairs("string", "str")))
            {
                template.Render(output, 10, true, '\0', "string");
                Assert.AreEqual("", output.ToString());
            }
        }
        [TestMethod()]
        public void TestCodeWithArguments()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new CSharp(), "{%?b%}{%@a%}test{%=c%}{%?!b%}{%resultString+=str;%}", null, new Pairs("int ", "a"), new Pairs("bool", "b"), new Pairs("char", "c"), new Pairs("string", "str")))
            {
                template.Render(output, 2, true, '\0', "string");
                Assert.AreEqual("testtest\0", output.ToString());
            }
        }
        [TestMethod()]
        public void TestCodeWithEnutherArguments()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new CSharp(), "{%?b%}{%@a%}test{%=c%}{%?!b%}{%resultString+=str;%}", null, new Pairs("int ", "a"), new Pairs("bool", "b"), new Pairs("char", "c"), new Pairs("string", "str")))
            {
                template.Render(output, 2, false, 'a', "string");
                Assert.AreEqual("astring", output.ToString());
            }
        }




        [TestMethod]
        public void TestLongestUnicode()
        {
            StringBuilder buffer = new StringBuilder();
            for (int i = Char.MinValue; i < Char.MaxValue; i++)
            {
                buffer.Append((char)i);
            }
            string text = string.Join(string.Empty, Enumerable.Repeat(buffer.ToString(), 1));

            TextWriter output = new StringWriter();
            using (Template template = new Template(new CSharp(), @"{%=@"""""""" + 42%}" + text))
            {
                template.Render(output);
                Assert.AreEqual("\"42" + text, output.ToString());
            }
        }

        [TestMethod()]
        public void TestHelloWorld()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new CSharp(), @"{%=a%}", null, new Pairs("string", "a")))
            {
                template.Render(output, "Hello world");
                Assert.AreEqual("Hello world", output.ToString());
            }
        }

        [TestMethod()]
        public void TestNewString()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new CSharp(), "\n"))
            {
                template.Render(output);
                Assert.AreEqual("\n", output.ToString());
            }
        }

        [TestMethod()]
        public void TestMaxCharString()
        {
            StringBuilder buffer = new StringBuilder();
            for (int i = Char.MinValue; i < Char.MaxValue; i++)
            {
                buffer.Append((char)i);
            }
            string text = string.Join(string.Empty, Enumerable.Repeat(buffer.ToString(), 8));
            TextWriter output = new StringWriter();
            using (Template template = new Template(new CSharp(), text))
            {
                template.Render(output);
                Assert.AreEqual(text, output.ToString());
            }
        }

    }



    [TestClass()]
    public class TemplateTestscJava
    {
        [TestMethod()]
        public void TestIf()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new Java(), "{%?true%}False", null))
            {
                template.Render(output);
                Assert.AreEqual("False", output.ToString());
            }
        }


        [TestMethod()]
        public void TestIfWithArgumentTrue()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new Java(), "{%?a%}true", null, new Pairs("boolean", "a")))
            {
                template.Render(output, true);
                Assert.AreEqual("true", output.ToString());
            }
        }
        [TestMethod()]
        public void TestIfWithArgumentsFalse()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new Java(), "{%?a%}true", null, new Pairs("boolean", "a")))
            {
                template.Render(output, false);
                Assert.AreEqual("", output.ToString());
            }
        }
        [TestMethod()]
        public void TestLoop()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new Java(), "{%@ 2+2%}{%resultString+=i;%}", null))
            {
                template.Render(output);
                Assert.AreEqual("0123", output.ToString());
            }
        }
        [TestMethod()]
        public void TestLoopWithArguments()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new Java(), "{%@a%}{%=12%}", null, new Pairs("int", "a")))
            {
                template.Render(output, 10 + (-5));
                Assert.AreEqual("1212121212", output.ToString());
            }
        }


        [TestMethod()]
        public void TestExpressionn()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new Java(), "{%= (10==1)%}", null))
            {
                template.Render(output);
                Assert.AreEqual("false", output.ToString());
            }
        }
        [TestMethod()]
        public void TestExpressionnWithParametrs()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new Java(), "{%= (10==a)%}", null, new Pairs("int", "a")))
            {
                template.Render(output, 10);
                Assert.AreEqual("true", output.ToString());
            }
        }
        [TestMethod()]
        public void TestText()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new Java(), "I want to say Hello!", null))
            {
                template.Render(output);
                Assert.AreEqual("I want to say Hello!", output.ToString());
            }
        }
        [TestMethod()]
        public void TestEmptyString()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new Java(), "", null))
            {
                template.Render(output);
                Assert.AreEqual("", output.ToString());
            }
        }
        //TODO: error failed
        [TestMethod()]
        public void TestCompileError()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new Java(), "{% {%}", null))
            {
                Exception exception = null;
                try
                {
                    template.Render(output);
                    Assert.AreEqual("", output.ToString());
                }
                catch (Exception e)
                {
                    exception = e;
                }
                Assert.IsNotNull(exception);
            }
        }




        [TestMethod]
        public void TestLongUnicode()
        {
            StringBuilder buffer = new StringBuilder();
            for (int i = Char.MinValue; i < Char.MaxValue / 1000; i++)
            {
                buffer.Append((char)i);
            }
            string text = string.Join(string.Empty, Enumerable.Repeat(buffer.ToString(), 1));

            StringWriter output = new StringWriter();
            using (Template template = new Template(new Java(), text))
            {
                template.Render(output);
                Assert.AreEqual(text, output.ToString());
            }
        }

        [TestMethod()]
        public void TestHelloWorld()
        {
            TextWriter output = new StringWriter();
            using (Template template = new Template(new Java(), @"{%=a%}", null, new Pairs("String", "a")))
            {
                template.Render(output, "Hello world");
                Assert.AreEqual("hello world", output.ToString());
            }
        }



    }

}