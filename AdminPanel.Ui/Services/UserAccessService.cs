using AdminPanel.Application.DTOs.Charity;
using AdminPanel.Ui.ViewModels;

namespace AdminPanel.Ui.Services
{
    //This service keeps current user status and implements helper methods in order to check roles
    public class UserAccessService
    {
        
        LoginValueViewModel _currentUser; 
        CharityDetailsDto _currentCharity;


        // Save current user information
        public void SetCurrentUser(LoginValueViewModel currentUser)
        {
            _currentUser = currentUser;
        }

        public void SetCurrentCharity(CharityDetailsDto currentCharity)
        {
            _currentCharity = currentCharity;
        }


        // Recieve current user info
        public LoginValueViewModel? GetCurrentUser()
        {
            return _currentUser;
        }


        public CharityDetailsDto? GetcurrentCharity()
        {
            return _currentCharity;
        }




        // Check if current user has logged in or not
        public bool IsAuthenticated()
        {
            return _currentUser != null;
        }



        // Methods for checking access level based on UserType
        // According to standard of this project we assume that:
        // 1 = SuperAdmin / Root
        // 2 = CharityAdmin / Admin
        // 3 = NormalUser / User
        public bool IsCharityAdmin()
        {
            return _currentUser != null && _currentUser.UserType == 2; 
        }

        public bool IsCharityUser()
        {
            return _currentUser != null && _currentUser.UserType == 3; 
        }

        public bool IsSystemAdmin()
        {
            return _currentUser != null && _currentUser.UserType == 1; 
        }


        public bool CanCreateCampaign()
        {
            return _currentUser != null &&
                   (_currentUser.UserType == 2 || _currentUser.UserType == 3);
        }


        // Helper method to check role with the help of role name (If needed somewhere)
        public bool HasRole(string roleName)
        {

            if (_currentUser == null)
            {
                return false;
            }

            return roleName.ToLower() switch
            {
                //Any one of the three roles can make one of the methods to happen
                "root" => IsSystemAdmin(),
                "admin" => IsCharityAdmin(),
                "user" => IsCharityUser(),
                _ => false
            };

        }

    }
}
