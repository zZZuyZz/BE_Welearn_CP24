using ServiceLayer.DTOs;
using ServiceLayer.Services.Interface;

namespace APIExtension.Validator
{
    //public interface IGroupValidator
    //{
    //    public Task<ValidatorResult> ValidateParams(GroupCreateDto dto);
    //    public Task<ValidatorResult> ValidateParams(GroupUpdateDto dto);
    //}
    //public class GroupValidator : BaseValidator, IGroupValidator
    //{
    //    private IServiceWrapper services;

    //    public GroupValidator(IServiceWrapper services)
    //    {
    //        this.services = services;
    //    }

    //    public async Task<ValidatorResult> ValidateParams(GroupCreateDto dto)
    //    {
    //        try
    //        {
    //            //Name
    //            if (dto.Name.Trim().Length == 0)
    //            {
    //                //validatorResult.Failures.Add("Thiếu tên nhóm");
    //                validatorResult.Add("Thiếu tên nhóm", nameof(dto.Name));
    //            }
    //            if (dto.Name.Trim().Length > 50)
    //            {
    //                //validatorResult.Failures.Add("Tên nhóm quá dài");
    //                validatorResult.Add("Tên nhóm quá dài", nameof(dto.Name));
    //            }
    //            ////Class
    //            //if (dto.ClassId < 6 || dto.ClassId > 12)
    //            //{
    //            //    //validatorResult.Failures.Add($"Lớp {dto.ClassId} không tồn tại");
    //            //    validatorResult.Add($"Lớp {dto.ClassId} không tồn tại", nameof(dto.Name));
    //            //}
    //            //Subject
    //            if (!dto.SubjectIds.Any())
    //            {
    //                //validatorResult.Failures.Add("Thiếu môn học");
    //                validatorResult.Add("Thiếu môn học", "subjects");
    //            }
    //            if (dto.SubjectIds.Any(id=> !services.Subjects.IsExistAsync((int)id).Result))
    //            {
    //                //validatorResult.Failures.Add($"Môn học không tồn tại");
    //                validatorResult.Add($"Môn học không tồn tại", "subjects");
    //            }
    //            //if (dto.SubjectIds.Any(id => (int)id < 1 || (int)id > 13))
    //            //{
    //            //    //validatorResult.Failures.Add($"Môn học không tồn tại");
    //            //    validatorResult.Add($"Môn học không tồn tại", "subjects");
    //            //}

    //        }
    //        catch (Exception ex)
    //        {
    //            //validatorResult.Failures.Add(ex.ToString());
    //            validatorResult.Add(ex.ToString());
    //        }
    //        return validatorResult;
    //    }

    //    public async Task<ValidatorResult> ValidateParams(GroupUpdateDto dto)
    //    {
    //        try
    //        {
    //            //Name
    //            //Nếu null thì ko update
    //            if (dto.Name != null)
    //            {
    //                if (dto.Name.Trim().Length == 0)
    //                {
    //                    //validatorResult.Failures.Add("Thiếu tên nhóm");
    //                    validatorResult.Add("Thiếu tên nhóm", nameof(dto.Name));
    //                }
    //                if (dto.Name.Trim().Length > 50)
    //                {
    //                    //validatorResult.Failures.Add("Tên nhóm quá dài");
    //                    validatorResult.Add("Tên nhóm quá dài", nameof(dto.Name));
    //                }
    //            }
    //            //Class
    //            //if (dto != null)
    //            //{
    //            //    if (dto.ClassId < 6 || dto.ClassId > 12)
    //            //    {
    //            //        validatorResult.Failures.Add($"Lớp {dto.ClassId} không tồn tại");
    //            //        validatorResult.Add($"Lớp {dto.ClassId} không tồn tại");
    //            //    }
    //            //}
    //            //Subject
    //            if (dto.SubjectIds != null)
    //            {
    //                if (!dto.SubjectIds.Any())
    //                {
    //                    validatorResult.Failures.Add("Thiếu môn học");
    //                    validatorResult.Add("Thiếu môn học");
    //                }
    //                if (dto.SubjectIds.Any(id => (int)id < 1 || (int)id > 13))
    //                {
    //                    validatorResult.Failures.Add($"Môn học không tồn tại");
    //                    validatorResult.Add($"Môn học không tồn tại");
    //                }
    //                if (dto.SubjectIds.Any(id => !services.Subjects.IsExistAsync((int)id).Result))
    //                {
    //                    //validatorResult.Failures.Add($"Môn học không tồn tại");
    //                    validatorResult.Add($"Môn học không tồn tại", "subjects");
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            //validatorResult.Failures.Add(ex.ToString());
    //            validatorResult.Add(ex.ToString());
    //        }
    //        return validatorResult;
    //    }
    //}
    public static class GroupValidatorExtension
    {
        public static async Task<ValidatorResult> ValidateParams(this ValidatorResult validatorResult, IServiceWrapper services, GroupCreateDto dto)
        {
            try
            {
                //Name
                if (dto.Name.Trim().Length == 0)
                {
                    //validatorResult.Failures.Add("Thiếu tên nhóm");
                    validatorResult.Add("Thiếu tên nhóm", nameof(dto.Name));
                }
                if (dto.Name.Trim().Length > 50)
                {
                    //validatorResult.Failures.Add("Tên nhóm quá dài");
                    validatorResult.Add("Tên nhóm quá dài", nameof(dto.Name));
                }
                ////Class
                //if (dto.ClassId < 6 || dto.ClassId > 12)
                //{
                //    //validatorResult.Failures.Add($"Lớp {dto.ClassId} không tồn tại");
                //    validatorResult.Add($"Lớp {dto.ClassId} không tồn tại", nameof(dto.Name));
                //}
                //Subject
                if (!dto.SubjectIds.Any())
                {
                    //validatorResult.Failures.Add("Thiếu môn học");
                    validatorResult.Add("Thiếu môn học", "subjects");
                }
                if (dto.SubjectIds.Any(id => !services.Subjects.IsExistAsync((int)id).Result))
                {
                    //validatorResult.Failures.Add($"Môn học không tồn tại");
                    validatorResult.Add($"Môn học không tồn tại", "subjects");
                }
                //if (dto.SubjectIds.Any(id => (int)id < 1 || (int)id > 13))
                //{
                //    //validatorResult.Failures.Add($"Môn học không tồn tại");
                //    validatorResult.Add($"Môn học không tồn tại", "subjects");
                //}

            }
            catch (Exception ex)
            {
                //validatorResult.Failures.Add(ex.ToString());
                validatorResult.Add(ex.ToString());
            }
            return validatorResult;
        }

        public static async Task<ValidatorResult> ValidateParams(this ValidatorResult validatorResult, IServiceWrapper services, GroupUpdateDto dto)
        {
            try
            {
                //Name
                //Nếu null thì ko update
                if (dto.Name != null)
                {
                    if (dto.Name.Trim().Length == 0)
                    {
                        //validatorResult.Failures.Add("Thiếu tên nhóm");
                        validatorResult.Add("Thiếu tên nhóm", nameof(dto.Name));
                    }
                    if (dto.Name.Trim().Length > 50)
                    {
                        //validatorResult.Failures.Add("Tên nhóm quá dài");
                        validatorResult.Add("Tên nhóm quá dài", nameof(dto.Name));
                    }
                }
                //Class
                //if (dto != null)
                //{
                //    if (dto.ClassId < 6 || dto.ClassId > 12)
                //    {
                //        validatorResult.Failures.Add($"Lớp {dto.ClassId} không tồn tại");
                //        validatorResult.Add($"Lớp {dto.ClassId} không tồn tại");
                //    }
                //}
                //Subject
                if (dto.SubjectIds != null)
                {
                    if (!dto.SubjectIds.Any())
                    {
                        validatorResult.Failures.Add("Thiếu môn học");
                        validatorResult.Add("Thiếu môn học");
                    }
                    if (dto.SubjectIds.Any(id => (int)id < 1 || (int)id > 13))
                    {
                        validatorResult.Failures.Add($"Môn học không tồn tại");
                        validatorResult.Add($"Môn học không tồn tại");
                    }
                    if (dto.SubjectIds.Any(id => !services.Subjects.IsExistAsync((int)id).Result))
                    {
                        //validatorResult.Failures.Add($"Môn học không tồn tại");
                        validatorResult.Add($"Môn học không tồn tại", "subjects");
                    }
                }
            }
            catch (Exception ex)
            {
                //validatorResult.Failures.Add(ex.ToString());
                validatorResult.Add(ex.ToString());
            }
            return validatorResult;
        }
    }
}