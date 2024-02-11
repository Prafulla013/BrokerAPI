using Common.Enumerations;

namespace Infrastructure.Common
{
    public static class GetModuleGroupAction
    {
        public static ModuleGroup GetModule(string controller)
        {
            switch (controller)
            {
                case "Dashboards":
                    return ModuleGroup.Dashboard;
                case "Employees":
                    return ModuleGroup.Employee;
                case "Users":
                    return ModuleGroup.UserAccess;
                case "Roles":
                    return ModuleGroup.Role;
                case "Accounts":
                    return ModuleGroup.Account;
                default:
                    return 0;
            }
        }

        public static ModuleAction GetAction(string action)
        {
            switch (action)
            {
                case "List":
                case "Get":
                case "ChangeLogs":
                case "UserActivityLogs":
                    return ModuleAction.Read;
                case "Create":
                case "CreateUser":
                    return ModuleAction.Create;
                case "Update":
                case "Upsert":
                    return ModuleAction.Update;
                case "Delete":
                    return ModuleAction.Delete;
                case "Reinvite":
                    return ModuleAction.Reinvite;
                case "Login":
                    return ModuleAction.Login;
                case "Activate":
                    return ModuleAction.Activate;
                case "RequestResetPassword":
                case "ResetPassword":
                    return ModuleAction.ResetPassword;
                case "RefreshToken":
                    return ModuleAction.RefreshToken;
                default:
                    return 0;
            }
        }
    }
}
