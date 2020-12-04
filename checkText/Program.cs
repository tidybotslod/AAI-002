using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using AAI;

namespace checkText
{
    public class Program
    {
        public void PromptForInput(StreamWriter writer)
        {
            KeySentiments server = new KeySentiments();
            do
            {
                Console.Write("Enter text <empty to quit>: ");
                string inputLine = Console.ReadLine();
                if (inputLine == null || inputLine.Length == 0)
                {
                    break;
                }
                else
                {
                    server.Sentiment(inputLine, writer);
                }
            } while (true);
        }

        public void StreamData(StreamReader reader, StreamWriter writer)
        {
            KeySentiments server = new KeySentiments();
            do
            {
                string inputLine = reader.ReadLine();
                server.Sentiment(inputLine, writer);
            } while (reader.EndOfStream == false);
        }

        static void Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                new Option<string>(
                    new string [] { "-i", "--inputFile" },
                    description: "Optionally specify an input file, if no file is specified read from stdin"),
                new Option<string>(
                     new string [] { "-o", "--outputFile" },
                     description: "Optionally specify an output file, if no file is specified write to stdout"),
                new Option<bool>(
                     new string [] { "-p", "--prompt" },
                     description: "Optionally prompt for input from the command line")
            };

            rootCommand.Description = "Analyze text for key words or for measuring sentiment.  This could be used to categorize the text and maintain the statistcs for how well a topic or product is perceived. Writes out the sentiment, positive score, negative score, neutral score and finally the keywords found.";

            rootCommand.Handler = CommandHandler.Create<string, string, bool>((inputFile, outputFile, prompt) =>
            {
                StreamReader input = null;
                StreamWriter output = null;

                Program azureAccess = new Program();
                if (outputFile != null)
                {
                    output = File.AppendText(outputFile);
                }
                else
                {
                    output = new StreamWriter(Console.OpenStandardOutput(), Console.OutputEncoding);
                }

                if (prompt)
                {
                    azureAccess.PromptForInput(output);
                }
                else
                {
                    if (inputFile != null)
                    {
                        input = File.OpenText(inputFile);
                    }
                    else
                    {
                        input = new StreamReader(Console.OpenStandardInput(), Console.InputEncoding);
                    }
                    azureAccess.StreamData(input, output);
                }
            });

            rootCommand.InvokeAsync(args).Wait();
        }
    }
}
