using MediatR;
using Titan_Fitness.Application_layer.DTOS;

namespace Titan_Fitness.Application_layer.Features.Members.Commands
{
    // 1. Definition of the Command
    public record RenewMembershipCommand(RenewMembershipDto Dto) : IRequest<int>;

    // 2. Handler inside the same file
    public class RenewMembershipCommandHandler : IRequestHandler<RenewMembershipCommand, int>
    {
        // أدخلي الـ DbContext أو الـ Repository الخاص بكِ هنا عبر الـ Constructor
        public RenewMembershipCommandHandler()
        {
        }

        public async Task<int> Handle(RenewMembershipCommand request, CancellationToken cancellationToken)
        {
            var memberId = request.Dto.MemberId;

            // TODO: اكيبي منطق تجديد الاشتراك هنا:
            // 1. جلب آخر اشتراك للمشترك من قاعدة البيانات
            // 2. إنشاء اشتراك جديد بنفس تفاصيل الخطة الكائنة
            // 3. حفظ التغييرات في قاعدة البيانات

            int newMembershipId = 1; // إرجاع معرّف الاشتراك الجديد

            return await Task.FromResult(newMembershipId);
        }
    }
}