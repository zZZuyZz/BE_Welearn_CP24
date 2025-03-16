using DataLayer.DbObject;
using DataLayer.Enums;
using ServiceLayer.DTOs;
using ServiceLayer.Services.Interface;

namespace APIExtension.Validator
{
    //public interface IGroupMemberValidator
    //{
    //    public Task<ValidatorResult> ValidateParamsAsync(GroupMemberInviteCreateDto? dto, int leaderId);
    //    public Task<ValidatorResult> ValidateParamsAsync(GroupMemberRequestCreateDto? dto);

    //}

    //public class GroupMemberValidator : BaseValidator, IGroupMemberValidator
    //{
    //    private IServiceWrapper services;

    //    public GroupMemberValidator(IServiceWrapper services)
    //    {
    //        this.services = services;
    //    }

    //    public async Task<ValidatorResult> ValidateParamsAsync(GroupMemberInviteCreateDto? dto, int leaderId)
    //    {
    //        try
    //        {
    //            if (!await services.Groups.IsStudentLeadingGroupAsync(leaderId, dto.GroupId))
    //            {
    //                //validatorResult.Failures.Add("Bạn không phải nhóm trưởng nhóm này");
    //                validatorResult.Add("Bạn không phải nhóm trưởng nhóm này", ValidateErrType.Role);
    //            }
    //            GroupMember exsited = await services.GroupMembers.GetGroupMemberOfStudentAndGroupAsync<GroupMember>(dto.AccountId, dto.GroupId);
    //            if (exsited != null)
    //            {
    //                switch (exsited.MemberRole)
    //                {
    //                    case GroupMemberRole.Leader:
    //                        {
    //                            //validatorResult.Failures.Add("Người dùng đã tham gia nhóm này");
    //                            validatorResult.Add("Người dùng đã tham gia nhóm này", ValidateErrType.Role);
    //                            break;
    //                        }
    //                    case GroupMemberRole.Member:
    //                        {
    //                            //validatorResult.Failures.Add("Người dùng đã tham gia nhóm này");
    //                            validatorResult.Add("Người dùng đã tham gia nhóm này", ValidateErrType.Role);
    //                            break;
    //                        }
    //                    default:
    //                        {
    //                            //validatorResult.Failures.Add("Người dùng đã có liên quan đến nhóm");
    //                            validatorResult.Add("Người dùng đã có liên quan đến nhóm", ValidateErrType.Role);
    //                            break;
    //                        }
    //                }
    //                if (await services.GroupMembers.AnyInviteAsync(dto.AccountId, dto.GroupId))
    //                {
    //                    //validatorResult.Failures.Add("Người dùng đã được mời vào nhóm");
    //                    validatorResult.Add("Người dùng đã được mời vào nhóm", ValidateErrType.Role);
    //                }
    //                if (await services.GroupMembers.AnyRequestAsync(dto.AccountId, dto.GroupId))
    //                {
    //                    validatorResult.Add("Người dùng đã xin vào nhóm", ValidateErrType.Role);
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            validatorResult.Failures.Add(ex.ToString());
    //        }
    //        return validatorResult;
    //    }

    //    public async Task<ValidatorResult> ValidateParamsAsync(GroupMemberRequestCreateDto? dto)
    //    {
    //        try
    //        {
    //            if (await services.Groups.IsStudentJoiningGroupAsync(dto.AccountId, dto.GroupId))
    //            {
    //                validatorResult.Add("Bạn đã tham gia nhóm này", ValidateErrType.Role);
    //            }
    //            GroupMember exsited = await services.GroupMembers.GetGroupMemberOfStudentAndGroupAsync<GroupMember>(dto.AccountId, dto.GroupId);
    //            if (exsited != null)
    //            {
    //                switch (exsited.MemberRole)
    //                {
    //                    case GroupMemberRole.Leader:
    //                        {
    //                            validatorResult.Add("Người dùng đã tham gia nhóm này", ValidateErrType.Role);
    //                            break;
    //                        }
    //                    case GroupMemberRole.Member:
    //                        {
    //                            validatorResult.Add("Người dùng đã tham gia nhóm này", ValidateErrType.Role);
    //                            break;
    //                        }
    //                    default:
    //                        {
    //                            validatorResult.Add("Người dùng đã có liên quan đến nhóm", ValidateErrType.Role);
    //                            break;
    //                        }
    //                }
    //                if (await services.GroupMembers.AnyInviteAsync(dto.AccountId, dto.GroupId))
    //                {
    //                    //validatorResult.Failures.Add("Người dùng đã được mời vào nhóm");
    //                    validatorResult.Add("Người dùng đã được mời vào nhóm", ValidateErrType.Role);
    //                }
    //                if (await services.GroupMembers.AnyRequestAsync(dto.AccountId, dto.GroupId))
    //                {
    //                    //validatorResult.Failures.Add("Người dùng đã xin vào nhóm");
    //                    validatorResult.Add("Người dùng đã xin vào nhóm", ValidateErrType.Role);
    //                }
    //                if (await services.GroupMembers.AnyInviteAsync(dto.AccountId, dto.GroupId))
    //                {
    //                    //validatorResult.Failures.Add("Người dùng đã được mời vào nhóm");
    //                    validatorResult.Add("Người dùng đã được mời vào nhóm", ValidateErrType.Role);
    //                }
    //                if (await services.GroupMembers.AnyRequestAsync(dto.AccountId, dto.GroupId))
    //                {
    //                    validatorResult.Add("Người dùng đã xin vào nhóm", ValidateErrType.Role);
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
    public static class GroupMemberValidatorExtension 
    {
        public static async Task<ValidatorResult> ValidateParamsAsync(this ValidatorResult validatorResult, IServiceWrapper services, GroupMemberInviteCreateDto? dto, int leaderId)
        {
            try
            {
                if (!await services.Groups.IsStudentLeadingGroupAsync(leaderId, dto.GroupId))
                {
                    //validatorResult.Failures.Add("Bạn không phải nhóm trưởng nhóm này");
                    validatorResult.Add("Bạn không phải nhóm trưởng nhóm này", ValidateErrType.Role);
                }
                GroupMember exsited = await services.GroupMembers.GetGroupMemberOfStudentAndGroupAsync<GroupMember>(dto.AccountId, dto.GroupId);
                if (exsited != null)
                {
                    switch (exsited.MemberRole)
                    {
                        case GroupMemberRole.Leader:
                            {
                                //validatorResult.Failures.Add("Người dùng đã tham gia nhóm này");
                                validatorResult.Add("Người dùng đã tham gia nhóm này", ValidateErrType.Role);
                                break;
                            }
                        case GroupMemberRole.Member:
                            {
                                //validatorResult.Failures.Add("Người dùng đã tham gia nhóm này");
                                validatorResult.Add("Người dùng đã tham gia nhóm này", ValidateErrType.Role);
                                break;
                            }
                        default:
                            {
                                //validatorResult.Failures.Add("Người dùng đã có liên quan đến nhóm");
                                validatorResult.Add("Người dùng đã có liên quan đến nhóm", ValidateErrType.Role);
                                break;
                            }
                    }
                    if (await services.GroupMembers.AnyInviteAsync(dto.AccountId, dto.GroupId))
                    {
                        //validatorResult.Failures.Add("Người dùng đã được mời vào nhóm");
                        validatorResult.Add("Người dùng đã được mời vào nhóm", ValidateErrType.Role);
                    }
                    if (await services.GroupMembers.AnyRequestAsync(dto.AccountId, dto.GroupId))
                    {
                        validatorResult.Add("Người dùng đã xin vào nhóm", ValidateErrType.Role);
                    }
                }
            }
            catch (Exception ex)
            {
                validatorResult.Failures.Add(ex.ToString());
            }
            return validatorResult;
        }

        public static async Task<ValidatorResult> ValidateParamsAsync(this ValidatorResult validatorResult, IServiceWrapper services, GroupMemberRequestCreateDto? dto)
        {
            try
            {
                if (await services.Groups.IsStudentJoiningGroupAsync(dto.AccountId, dto.GroupId))
                {
                    validatorResult.Add("Bạn đã tham gia nhóm này", ValidateErrType.Role);
                }
                GroupMember exsited = await services.GroupMembers.GetGroupMemberOfStudentAndGroupAsync<GroupMember>(dto.AccountId, dto.GroupId);
                if (exsited != null)
                {
                    switch (exsited.MemberRole)
                    {
                        case GroupMemberRole.Leader:
                            {
                                validatorResult.Add("Người dùng đã tham gia nhóm này", ValidateErrType.Role);
                                break;
                            }
                        case GroupMemberRole.Member:
                            {
                                validatorResult.Add("Người dùng đã tham gia nhóm này", ValidateErrType.Role);
                                break;
                            }
                        default:
                            {
                                validatorResult.Add("Người dùng đã có liên quan đến nhóm", ValidateErrType.Role);
                                break;
                            }
                    }
                    if (await services.GroupMembers.AnyInviteAsync(dto.AccountId, dto.GroupId))
                    {
                        //validatorResult.Failures.Add("Người dùng đã được mời vào nhóm");
                        validatorResult.Add("Người dùng đã được mời vào nhóm", ValidateErrType.Role);
                    }
                    if (await services.GroupMembers.AnyRequestAsync(dto.AccountId, dto.GroupId))
                    {
                        //validatorResult.Failures.Add("Người dùng đã xin vào nhóm");
                        validatorResult.Add("Người dùng đã xin vào nhóm", ValidateErrType.Role);
                    }
                    if (await services.GroupMembers.AnyInviteAsync(dto.AccountId, dto.GroupId))
                    {
                        //validatorResult.Failures.Add("Người dùng đã được mời vào nhóm");
                        validatorResult.Add("Người dùng đã được mời vào nhóm", ValidateErrType.Role);
                    }
                    if (await services.GroupMembers.AnyRequestAsync(dto.AccountId, dto.GroupId))
                    {
                        validatorResult.Add("Người dùng đã xin vào nhóm", ValidateErrType.Role);
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