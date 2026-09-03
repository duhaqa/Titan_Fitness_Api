using System;
using System.Linq;
using System.Threading.Tasks;
using Titan_Fitness.Data.DB;
using Titan_Fitness.Domain.Entites;
using Titan_Fitness.Domain.Value_object;

namespace Titan_Fitness.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(DB_context context)
        {
            // إنشاء قاعدة البيانات إن لم تكن موجودة
            await context.Database.EnsureCreatedAsync();

            // 1. إضافة خطط الاشتراك (Plans) إن كانت فارغة
            if (!context.Plans.Any())
            {
                // إذا كان لدى Plan طريقة Create معقدة، يمكنك تعديل المعاملات هنا حسب التوقيع المطلوب
                // أو إلغاء التعليق وتعبئة البيانات التجريبية تلقائياً
            }

            // 2. إضافة المدربين (Trainers) إن كانت فارغة
            if (!context.Trainers.Any())
            {
                // إمكانية الإضافة فور تجهيز التوقيع المباشر
            }

            // 3. إضافة الأعضاء (Members) إن كانت فارغة
            if (!context.Members.Any())
            {
                // إمكانية الإضافة فور تجهيز التوقيع المباشر
            }

            await context.SaveChangesAsync();
        }
    }
}