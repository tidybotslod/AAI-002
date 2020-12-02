using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using checkText;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void testStreamData()
        {
            string input = "The quick brown fox jumps over the lazy dog";
            string answer = "Negative, 0.00, 0.99, 0.01, \"quick brown fox jumps\", \"lazy dog\"";

            Program app = new Program();
            using (MemoryStream source = new MemoryStream())
            {
                // Create a stream containing the test phrase
                StreamWriter writer = new StreamWriter(source, Console.OutputEncoding);
                writer.WriteLine(input);
                writer.Flush();
                source.Seek(0, SeekOrigin.Begin);

                using (MemoryStream sink = new MemoryStream())
                {

                    // Create a TextReader over the stream, data will be pulled here
                    StreamReader reader = new StreamReader(source, Console.InputEncoding);

                    // Create an output Stream to get the data
                    StreamWriter result = new StreamWriter(sink, Console.OutputEncoding);

                    // Get the results
                    app.StreamData(reader, result);

                    // Evaulate result
                    sink.Seek(0, SeekOrigin.Begin);
                    StreamReader output = new StreamReader(sink, Console.InputEncoding);
                    string value = output.ReadLine();
                    Assert.AreEqual(answer, value);
                }
            }
        }

        [TestMethod]
        public void testPromptForInput()
        {
            // Testing input from the console is accomplished by replacing the input stream use in Console
            // with a memory stream containing one line.

            string input = "The quick brown fox jumps over the lazy dog";
            string answer = "Negative, 0.00, 0.99, 0.01, \"quick brown fox jumps\", \"lazy dog\"";

            Program app = new Program();
            using (MemoryStream source = new MemoryStream())
            {
                using (MemoryStream sink = new MemoryStream())
                {
                    try
                    {
                        // Create a stream containing the test phrase
                        StreamWriter writer = new StreamWriter(source, Console.OutputEncoding);
                        writer.WriteLine(input);
                        writer.Flush();
                        source.Seek(0, SeekOrigin.Begin);

                        // Create a TextReader over the memory stream and replace the input stream used by the console
                        TextReader reader = new StreamReader(source, Console.InputEncoding);
                        Console.SetIn(reader);

                        // Create a Writer that will stream out the results
                        StreamWriter result = new StreamWriter(sink, Console.OutputEncoding);

                        // Get the results
                        app.PromptForInput(result);

                        // Test if the output lines match what is expected
                        sink.Seek(0, SeekOrigin.Begin);
                        StreamReader output = new StreamReader(sink, Console.InputEncoding);
                        string value = output.ReadLine();
                        Assert.AreEqual(answer, value);
                    }
                    finally
                    {
                        // Reset Console so it does not affect other tests.
                        StreamReader reset = new StreamReader(Console.OpenStandardInput());
                        Console.SetIn(reset);
                    }
                }
            }
        }
    }
}
