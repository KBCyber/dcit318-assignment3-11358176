using System;
using System.Collections.Generic;
using System.IO;

public class InvalidScoreFormatException : Exception
{
    public InvalidScoreFormatException(string message) : base(message) { }
}

public class MissingFieldException : Exception
{
    public MissingFieldException(string message) : base(message) { }
}

public class Student
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public int Score { get; set; }

    public Student(int id, string fullName, int score)
    {
        Id = id;
        FullName = fullName;
        Score = score;
    }

    public string GetGrade()
    {
        if (Score >= 80 && Score <= 100) return "A";
        if (Score >= 70) return "B";
        if (Score >= 60) return "C";
        if (Score >= 50) return "D";
        return "F";
    }
}

public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        var students = new List<Student>();

        using (var reader = new StreamReader(inputFilePath))
        {
            string line;
            int lineNumber = 0;

            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;
                string[] parts = line.Split(',');

                if (parts.Length != 3)
                    throw new MissingFieldException($"Line {lineNumber}: Missing fields.");

                try
                {
                    int id = int.Parse(parts[0].Trim());
                    string name = parts[1].Trim();
                    if (string.IsNullOrWhiteSpace(name))
                        throw new MissingFieldException($"Line {lineNumber}: Full name is missing.");

                    if (!int.TryParse(parts[2].Trim(), out int score))
                        throw new InvalidScoreFormatException($"Line {lineNumber}: Invalid score format.");

                    students.Add(new Student(id, name, score));
                }
                catch (FormatException)
                {
                    throw new InvalidScoreFormatException($"Line {lineNumber}: Score is not a valid number.");
                }
            }
        }
        return students;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using (var writer = new StreamWriter(outputFilePath))
        {
            writer.WriteLine("===== Student Summary Report =====");
            foreach (var student in students)
            {
                writer.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
            }
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        string projectDir = AppDomain.CurrentDomain.BaseDirectory;
        string inputFilePath = Path.Combine(projectDir, "Data", "students.txt");
        string outputFilePath = Path.Combine(projectDir, "Data", "report.txt");

        var processor = new StudentResultProcessor();

        try
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Reading student data...");
            Console.ResetColor();

            var students = processor.ReadStudentsFromFile(inputFilePath);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Data successfully read. Writing report...");
            Console.ResetColor();

            processor.WriteReportToFile(students, outputFilePath);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Report generated successfully!");
            Console.WriteLine($"Location: {outputFilePath}");
            Console.ResetColor();
        }
        catch (FileNotFoundException)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: Input file not found.");
            Console.ResetColor();
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
        }
        catch (MissingFieldException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Unexpected error: {ex.Message}");
            Console.ResetColor();
        }
    }
}
