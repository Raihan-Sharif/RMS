using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SimRMS.Application.Models.Requests;
using SimRMS.Application.Validators;
using System.Reflection;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Dependency Injection for Application Layer
/// Author:      Md. Raihan Sharif
/// Purpose:     This class configures the dependency injection for the application layer.
/// Creation:    03/Aug/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// [Missing]
/// 
/// ===================================================================
/// </para>
/// </summary>
/// 

namespace SimRMS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // FluentValidation
        services.AddScoped<IValidator<UsrInfoRequest>, UsrInfoRequestValidator>();

        #region Company Validators
        services.AddScoped<IValidator<UpdateCompanyRequest>, UpdateCompanyRequestValidator>();
        services.AddScoped<IValidator<AuthorizeCompanyRequest>, AuthorizeCompanyRequestValidator>();
        services.AddScoped<IValidator<GetCompanyWorkflowListRequest>, GetCompanyWorkflowListRequestValidator>();
        #endregion

        #region Broker Branch Validators
        services.AddScoped<IValidator<CreateMstCoBrchRequest>, CreateMstCoBrchRequestValidator>();
        services.AddScoped<IValidator<UpdateMstCoBrchRequest>, UpdateMstCoBrchRequestValidator>();
        services.AddScoped<IValidator<DeleteMstCoBrchRequest>, DeleteMstCoBrchRequestValidator>();
        services.AddScoped<IValidator<AuthorizeMstCoBrchRequest>, AuthorizeMstCoBrchRequestValidator>();
        services.AddScoped<IValidator<GetBranchWorkflowListRequest>, GetBranchWorkflowListRequestValidator>();
        #endregion

        #region Trader Validators
        services.AddScoped<IValidator<CreateMstTraderRequest>, CreateMstTraderRequestValidator>();
        services.AddScoped<IValidator<UpdateMstTraderRequest>, UpdateMstTraderRequestValidator>();
        services.AddScoped<IValidator<DeleteMstTraderRequest>, DeleteMstTraderRequestValidator>();
        services.AddScoped<IValidator<AuthorizeMstTraderRequest>, AuthorizeMstTraderRequestValidator>();
        services.AddScoped<IValidator<GetTraderWorkflowListRequest>, GetTraderWorkflowListRequestValidator>();
        #endregion

        #region User Validators
        services.AddScoped<IValidator<CreateUserRequest>, CreateUserRequestValidator>();
        services.AddScoped<IValidator<UpdateUserRequest>, UpdateUserRequestValidator>();
        services.AddScoped<IValidator<DeleteUserRequest>, DeleteUserRequestValidator>();
        services.AddScoped<IValidator<GetUserWorkflowListRequest>, GetUserWorkflowListRequestValidator>();
        services.AddScoped<IValidator<AuthorizeUserRequest>, AuthorizeUserRequestValidator>();
        #endregion

        #region UserExposure Validators
        services.AddScoped<IValidator<CreateUserExposureRequest>, CreateUserExposureRequestValidator>();
        services.AddScoped<IValidator<UpdateUserExposureRequest>, UpdateUserExposureRequestValidator>();
        services.AddScoped<IValidator<DeleteUserExposureRequest>, DeleteUserExposureRequestValidator>();
        services.AddScoped<IValidator<AuthorizeUserExposureRequest>, AuthorizeUserExposureRequestValidator>();
        services.AddScoped<IValidator<GetUserExposureWorkflowListRequest>, GetUserExposureWorkflowListRequestValidator>();
        #endregion

        #region OrderGroup Validators
        // Common validators
        services.AddScoped<IValidator<GetOrderGroupByCodeRequest>, GetOrderGroupByCodeRequestValidator>();
        services.AddScoped<IValidator<GetOrderGroupListRequest>, GetOrderGroupListRequestValidator>();
        
        // Master validators
        services.AddScoped<IValidator<CreateOrderGroupMasterRequest>, CreateOrderGroupMasterRequestValidator>();
        services.AddScoped<IValidator<UpdateOrderGroupMasterRequest>, UpdateOrderGroupMasterRequestValidator>();
        services.AddScoped<IValidator<DeleteOrderGroupMasterRequest>, DeleteOrderGroupMasterRequestValidator>();
        services.AddScoped<IValidator<AuthorizeOrderGroupMasterRequest>, AuthorizeOrderGroupMasterRequestValidator>();
        
        // Detail/User validators
        services.AddScoped<IValidator<AddUserToOrderGroupRequest>, AddUserToOrderGroupRequestValidator>();
        services.AddScoped<IValidator<UpdateOrderGroupUserRequest>, UpdateOrderGroupUserRequestValidator>();
        services.AddScoped<IValidator<RemoveUserFromGroupRequest>, RemoveUserFromGroupRequestValidator>();
        services.AddScoped<IValidator<AuthorizeOrderGroupDetailRequest>, AuthorizeOrderGroupDetailRequestValidator>();
        
        // Workflow validators
        services.AddScoped<IValidator<GetMasterGroupWorkflowListRequest>, GetMasterGroupWorkflowListRequestValidator>();
        services.AddScoped<IValidator<GetDetailGroupWorkflowListRequest>, GetDetailGroupWorkflowListRequestValidator>();
        
        // Legacy validators (required for service)
        services.AddScoped<IValidator<CreateOrderGroupRequest>, CreateOrderGroupRequestValidator>();
        services.AddScoped<IValidator<UpdateOrderGroupRequest>, UpdateOrderGroupRequestValidator>();
        services.AddScoped<IValidator<DeleteOrderGroupRequest>, DeleteOrderGroupRequestValidator>();
        services.AddScoped<IValidator<AuthorizeOrderGroupRequest>, AuthorizeOrderGroupRequestValidator>();
        #endregion

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: false);

        return services;
    }
}
