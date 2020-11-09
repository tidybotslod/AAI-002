using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using keySentiments;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        class TestCase
        {
            internal string[] input;
            internal string[] answer;
        }

        private static TestCase[] testCases = {
            new TestCase {
                input  = new string[] { "The quick brown fox jumps over the lazy dog" },
                answer = new string[] { "Negative, 0.00, 0.99, 0.01, \"quick brown fox jumps\", \"lazy dog\"" }
            }
        };

        private void loadStream(Stream load, int testNumber)
        {
            StreamWriter writer = new StreamWriter(load, Console.OutputEncoding);
            for (int line = 0; line < testCases[testNumber].input.Length; line++)
            {
                writer.WriteLine(testCases[testNumber].input[line]);
                writer.Flush();
                load.Seek(0, SeekOrigin.Begin);
            }
        }

        private void checkOutput(Stream sink, int testNumber)
        {
            // Rewind the output, allows it to be read in.
            sink.Seek(0, SeekOrigin.Begin);

            // Read back the results line by line and compare them to what is expected.
            StreamReader output = new StreamReader(sink, Console.InputEncoding);
            for (int line = 0; line < testCases[testNumber].answer.Length; line++)
            {
                string value = output.ReadLine();
                // Make sure the same value came back;
                Assert.AreEqual(testCases[testNumber].answer[line], value);
            }
        }

        [TestMethod]
        [DataRow(0, DisplayName = "StreamLine1")]
        public void testStreamData(int testNumber)
        {
            Program app = new Program();
            using (MemoryStream source = new MemoryStream())
            {
                using (MemoryStream sink = new MemoryStream())
                {
                    // Write out the test case to a stream
                    loadStream(source, testNumber);

                    // Create a TextReader over the stream, data will be pulled here
                    StreamReader reader = new StreamReader(source, Console.InputEncoding);

                    // Create an output Stream to get the data
                    StreamWriter result = new StreamWriter(sink, Console.OutputEncoding);

                    // Get the results
                    app.StreamData(reader, result);

                    // Evaulate result
                    checkOutput(sink, testNumber);
                }
            }
        }

        [TestMethod]
        [DataRow(0, DisplayName = "PromptForLine1")]

        public void testPromptForInput(int testNumber)
        {
            Program app = new Program();
            using (MemoryStream source = new MemoryStream())
            {
                using (MemoryStream sink = new MemoryStream())
                {
                    try
                    {
                        // Write out the test case to a stream 
                        loadStream(source, testNumber);

                        // Create a TextReader over the memory stream and replace the input stream used by the console
                        TextReader reader = new StreamReader(source, Console.InputEncoding);
                        Console.SetIn(reader);

                        // Create a Writer that will stream out the results
                        StreamWriter result = new StreamWriter(sink, Console.OutputEncoding);

                        // Get the results
                        app.PromptForInput(result);

                        // Test if the output lines match what is expected
                        checkOutput(sink, testNumber);
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
