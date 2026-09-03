using Titan_Fitness.Domain.Value_object;

namespace Titan_Fitness.Domain.Entites;
    

//using TitanFitness.Domain.ValueObjects;

    public class Trainer
    {
        public int Id { get; private set; } // a. Trainer Id                     
        public string Name { get; private set; } = null!; // b. Trainer name (max 100 char, required)                     
        public string? Email { get; private set; } // c. Email (max 100 char)                     
        public Phone Phone { get; private set; } = null!; // d. Phone (max 20 char)                     
        public bool IsActive { get; private set; } // e. Is active (true or false)                     

        private Trainer() { }

        private Trainer(string name, string? email, Phone phone, bool isActive)
        {
            Name = name;
            Email = email;
            Phone = phone;
            IsActive = isActive;
        }

        public static Trainer Create(string name, string? email, Phone phone, bool isActive = true)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("اسم المدرب مطلوب.");                      

        if (name.Length > 100)
                throw new ArgumentException("اسم المدرب يجب ألا يتجاوز 100 حرف.");                      

        if (email?.Length > 100)
                throw new ArgumentException("البريد الإلكتروني يجب ألا يتجاوز 100 حرف.");                      

        return new Trainer(name, email, phone, isActive);
        }

        public void UpdateProfile(string name, string? email, Phone phone)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
                throw new ArgumentException("اسم المدرب غير صالح.");                      

        if (email?.Length > 100)
                throw new ArgumentException("البريد الإلكتروني يجب ألا يتجاوز 100 حرف.");                      

        Name = name;
            Email = email;
            Phone = phone;
        }

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;
    }
