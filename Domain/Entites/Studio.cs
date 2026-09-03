namespace Titan_Fitness.Domain.Entites
{
    public class Studio
    {
        public int Id { get; private set; } // Studio Id   
        public string Name { get; private set; } = null!; // max 50 char, required   
        public int BranchId { get; private set; } // Branch Id   
        public int Capacity { get; private set; } // required, max people room holds   

        private Studio() { }

        public static Studio Create(string name, int branchId, int capacity)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > 50)
                throw new ArgumentException("اسم الصالة مطلوب ويجب ألا يتجاوز 50 حرفاً.");    

            if (branchId <= 0)
                throw new ArgumentException("معرف الفرع غير صالح.");

            if (capacity <= 0)
                throw new ArgumentException("السعة الاستيعابية يجب أن تكون أكبر من 0.");    

            return new Studio { Name = name, BranchId = branchId, Capacity = capacity };
        }

        public void Update(string name, int capacity)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > 50)
                throw new ArgumentException("اسم الصالة غير صالح.");    

            if (capacity <= 0)
                throw new ArgumentException("السعة الاستيعابية غير صالحة.");    

            Name = name;
            Capacity = capacity;
        }
    }
}