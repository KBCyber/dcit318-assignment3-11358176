using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthcareSystem
{
    public class Repository<T>
    {
        private readonly List<T> _items = new();

        public void Add(T item) => _items.Add(item);

        public List<T> GetAll() => new List<T>(_items);

        public T? GetById(Func<T, bool> predicate) => _items.FirstOrDefault(predicate);

        public bool Remove(Func<T, bool> predicate)
        {
            var item = _items.FirstOrDefault(predicate);
            if (item != null)
            {
                _items.Remove(item);
                return true;
            }
            return false;
        }
    }

    public class Patient
    {
        public int Id { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }

        public override string ToString() => $"ID: {Id} | {Name} | Age: {Age} | Gender: {Gender}";
    }

    public class Prescription
    {
        public int Id { get; }
        public int PatientId { get; }
        public string MedicationName { get; }
        public DateTime DateIssued { get; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }

        public override string ToString() => $"RxID: {Id} | Med: {MedicationName} | Date: {DateIssued:yyyy-MM-dd}";
    }

    public class HealthSystemApp
    {
        private Repository<Patient> _patientRepo = new();
        private Repository<Prescription> _prescriptionRepo = new();
        private Dictionary<int, List<Prescription>> _prescriptionMap = new();

        public void SeedData()
        {
            _patientRepo.Add(new Patient(1, "Augustine Ninyung", 30, "Male"));
            _patientRepo.Add(new Patient(2, "Samuel Manu", 45, "Male"));
            _patientRepo.Add(new Patient(3, "Abena Amponsahs", 29, "Female"));

            _prescriptionRepo.Add(new Prescription(1, 1, "Amoxicillin", DateTime.Now.AddDays(-10)));
            _prescriptionRepo.Add(new Prescription(2, 1, "Ibuprofen", DateTime.Now.AddDays(-5)));
            _prescriptionRepo.Add(new Prescription(3, 2, "Metformin", DateTime.Now.AddDays(-20)));
            _prescriptionRepo.Add(new Prescription(4, 3, "Lisinopril", DateTime.Now.AddDays(-15)));
            _prescriptionRepo.Add(new Prescription(5, 3, "Paracetamol", DateTime.Now.AddDays(-3)));
        }

        public void BuildPrescriptionMap()
        {
            _prescriptionMap.Clear();

            foreach (var prescription in _prescriptionRepo.GetAll())
            {
                if (!_prescriptionMap.ContainsKey(prescription.PatientId))
                {
                    _prescriptionMap[prescription.PatientId] = new List<Prescription>();
                }
                _prescriptionMap[prescription.PatientId].Add(prescription);
            }
        }

        public void PrintAllPatients()
        {
            Console.WriteLine("\n=== All Patients ===");
            foreach (var patient in _patientRepo.GetAll())
            {
                Console.WriteLine(patient);
            }
        }

        public void PrintPrescriptionsForPatient(int patientId)
        {
            if (_prescriptionMap.TryGetValue(patientId, out var prescriptions))
            {
                Console.WriteLine($"\nPrescriptions for Patient ID {patientId}:");
                foreach (var prescription in prescriptions)
                {
                    Console.WriteLine(prescription);
                }
            }
            else
            {
                Console.WriteLine($"\nNo prescriptions found for Patient ID {patientId}.");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var app = new HealthSystemApp();

            app.SeedData();
            app.BuildPrescriptionMap();

            app.PrintAllPatients();

            Console.Write("\nEnter Patient ID to view prescriptions: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                app.PrintPrescriptionsForPatient(id);
            }
            else
            {
                Console.WriteLine("Invalid input.");
            }

            Console.WriteLine("\n=== End of Program ===");
        }
    }
}
