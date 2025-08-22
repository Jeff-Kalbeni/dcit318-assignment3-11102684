using System;
using System.Collections.Generic;
using System.Linq;


namespace HealthcareManagementSystem
{
    public class Repository<T>
    {
        private readonly List<T> _items = new List<T>();
        public void Add(T item)
        {
            _items.Add(item);
        }

        public List<T> GetAll(){
            return new List<T>(_items);
        }

        public T? GetById(Func<T, bool> predicate)
        {
            return _items.FirstOrDefault(predicate);
        }

        public bool Remove(Func<T, bool> predicate)
        {
            var item = _items.FirstOrDefault(predicate);
            return item != null && _items.Remove(item);
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
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Age = age;
            Gender = gender ?? throw new ArgumentNullException(nameof(gender));
        }

        public override string ToString()
        {
            return $"Patient [Id={Id}, Name={Name}, Age={Age}, Gender={Gender}]";
        }
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
            MedicationName = medicationName ?? throw new ArgumentNullException(nameof(medicationName));
            DateIssued = dateIssued;
        }

        public override string ToString()
        {
            return $"Prescription [Id={Id}, PatientId={PatientId}, Medication={MedicationName}, Date={DateIssued}]";
        }
    }

    public class HealthSystemApp
    {
        private readonly Repository<Patient> _patientRepo = new Repository<Patient>();
        private readonly Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
        private readonly Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();

        public void BuildPrescriptionMap()
        {
            _prescriptionMap.Clear();
            var allPrescriptions = _prescriptionRepo.GetAll();
            foreach (var prescription in allPrescriptions)
            {
                if (!_prescriptionMap.ContainsKey(prescription.PatientId))
                {
                    _prescriptionMap[prescription.PatientId] = new List<Prescription>();

                }
                _prescriptionMap[prescription.PatientId].Add(prescription);
            }
        }

        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            return _prescriptionMap.ContainsKey(patientId) ? _prescriptionMap[patientId] : new List<Prescription>();
        }

        public void SeedData()
        {
            _patientRepo.Add(new Patient(1, "Kojo Kumah", 35, "Male"));
            _patientRepo.Add(new Patient(2, "Francis Tsum", 40, "Male"));
            _patientRepo.Add(new Patient(3, "Stacy Ansah", 20, "Female"));

            _prescriptionRepo.Add(new Prescription(1, 1, "Aspirin", DateTime.Now.AddDays(-10)));
            _prescriptionRepo.Add(new Prescription(2, 1, "Penicillin", DateTime.Now.AddDays(-7)));
            _prescriptionRepo.Add(new Prescription(3, 2, "Ibucap", DateTime.Now.AddDays(-4)));
            _prescriptionRepo.Add(new Prescription(4, 3, "Panadol", DateTime.Now.AddDays(-6)));
            _prescriptionRepo.Add(new Prescription(5, 2, "Paracetamol", DateTime.Now.AddDays(-3)));
        }

        public void PrintAllPatients()
        {
            var patients = _patientRepo.GetAll();
            Console.WriteLine("All Patients:");
            foreach (var patient in patients)
            {
                Console.WriteLine(patient);
            }
        }

        public void PrintPrescriptionsForPatient(int patientId)
        {
            var prescriptions = GetPrescriptionsByPatientId(patientId);
            if (prescriptions.Any())
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
        public static void Main ()
        {
            var app = new HealthSystemApp();

            app.SeedData();
            app.BuildPrescriptionMap();
            app.PrintAllPatients();
            app.PrintPrescriptionsForPatient(3);
        }
    }
}