using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;

namespace SchoolGradingSystem
{
    public class Student
    {
        public int Id { get; }
        public string FullName { get; }
        public int Score { get; }
        public Student(int id, string fullName, int score)
        {
            Id = id;
            FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
            Score = score;
        }

        public string GetGrade()
        {
            return Score switch
            {
                >= 80 and <= 100 => "A",
                >= 70 and <= 79 => "B",
                >= 60 and <= 69 => "C",
                >= 50 and <= 59 => "D",
                _ => "F"
            };
        }
    }

    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }

    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            var students = new List<Student>();
            using (var reader = new StreamReader(inputFilePath))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (string.IsNullOrEmpty(line)) continue;

                    var fields = line.Split(',');
                    if(fields.Length != 3)
                    {
                        throw new MissingFieldException($"Invalid number of fields in line: '{line}'. Expected 3, got {fields.Length}.");
                    }

                    try
                    {
                        int id = int.Parse(fields[0].Trim());
                        string fullName = fields[1].Trim();
                        int score = int.Parse(fields[2].Trim());

                        if(score < 0 || score > 100)
                            throw new InvalidScoreFormatException($"Score {score} is out of range (0-100) for student: {fullName}");

                        students.Add(new Student(id, fullName, score));
                        
                    }
                    catch (FormatException)
                    {
                        throw new InvalidScoreFormatException($"Invalid score format in line: '{line}'.");
                    }
                }
            }
            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using (var writer = new StreamWriter(outputFilePath, append:false))
            {
                if (students.Count == 0)
                {
                    writer.WriteLine("No valid student records to report.");
                    return;
                }
                foreach (var student in students)
                {
                    string summary = $"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}";
                    writer.WriteLine(summary);
                }
            }
        }
    }

    class Program
    {
        static void Main()
        {
            var processor = new StudentResultProcessor();
            string inputFilePath = "C:\\Dkp Files\\Lecture Slides\\DCIT 318\\dcit318-assignment3-11102684\\SchoolGradingSystem\\SchoolGradingSystem\\students.txt";
            string outputFilePath = "C:\\Dkp Files\\Lecture Slides\\DCIT 318\\dcit318-assignment3-11102684\\SchoolGradingSystem\\SchoolGradingSystem\\report.txt";

            try
            {
                var students = processor.ReadStudentsFromFile(inputFilePath);
                processor.WriteReportToFile(students, outputFilePath);

                Console.WriteLine("Report generated successfully at " + outputFilePath);
            } catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error: Input file not found - {ex.Message}");
            } catch(InvalidScoreFormatException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            } catch (MissingFieldException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            } catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }
    }
}