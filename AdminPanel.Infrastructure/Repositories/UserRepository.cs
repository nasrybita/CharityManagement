using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminPanel.Application.DTOs.Users;
using AdminPanel.Application.DTOs.User;
using AdminPanel.Application.Interfaces;
using AdminPanel.Core.Enums;

using Microsoft.EntityFrameworkCore;
using AdminPanel.Infrastructure.Persistence.Data;

namespace AdminPanel.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
      

        private readonly AdminPanelDbContext _context;


        public UserRepository(AdminPanelDbContext context)
        {
            _context = context;
        }



        public async Task<List<UserDto>> GetAllAsync()
        {
            var users =
                await
                (
                    from user in _context.AdminUsers

                    join charity in _context.Charities
                    on user.CharityId equals charity.Id into charities

                    from charity in charities.DefaultIfEmpty()

                    where !user.IsDeleted

                    select new UserDto
                    {
                        Id = user.Id,

                        Name = user.Name,

                        UserName = user.UserName,

                        Mobile = user.Mobile,


                        CharityId = user.CharityId,

                        CharityName =
                            charity != null
                            ? charity.Name
                            : null,


                        CreatedAt = user.CreatedAt,


                        IsAdmin = user.IsAdmin,

                        IsRoot = user.IsRoot,

                        Sex = user.Sex
                    }


                ).ToListAsync();


            return users;
        }




        public async Task<AdminUserDetailsDto?> GetByIdAsync(int id)
        {

            var user =
                await
                (
                    from u in _context.AdminUsers

                    join charity in _context.Charities
                    on u.CharityId equals charity.Id into charities

                    from charity in charities.DefaultIfEmpty()


                    where u.Id == id &&
                          !u.IsDeleted


                    select new AdminUserDetailsDto
                    {
                        Id = u.Id,

                        Name = u.Name,

                        UserName = u.UserName,

                        Mobile = u.Mobile,


                        CharityId = u.CharityId,


                        CharityName =
                            charity != null
                            ? charity.Name
                            : null,


                        CreatedAt = u.CreatedAt,


                        IsAdmin = u.IsAdmin,

                        IsRoot = u.IsRoot,

                        Sex = u.Sex
                    }


                )
                .FirstOrDefaultAsync();


            return user;
        }






        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            return await
            (
                from user in _context.AdminUsers

                join charity in _context.Charities
                on user.CharityId equals charity.Id into charities

                from charity in charities.DefaultIfEmpty()


                where user.Id == id &&
                      !user.IsDeleted


                //select new UserDto
                //{
                //    Id = user.Id,

                //    Name = user.Name,

                //    UserName = user.UserName,

                //    Mobile = user.Mobile,


                //    CharityId = user.CharityId,


                //    CharityName =
                //        charity != null
                //        ? charity.Name
                //        : null,


                //    IsAdmin = user.IsAdmin,

                //    IsRoot = user.IsRoot,

                //    Sex = user.Sex,

                //    CreatedAt = user.CreatedAt
                //}




                select new UserDto
                {
                    Id = user.Id,

                    Name = user.Name,

                    UserName = user.UserName,

                    Mobile = user.Mobile,

                    CharityId = user.CharityId,

                    CharityName = charity != null
                ? charity.Name
                : null,

                    CreatedAt = user.CreatedAt,

                    IsAdmin = user.IsAdmin,

                    IsRoot = user.IsRoot,

                    Sex = user.Sex,

                    UserType =
                user.IsRoot
                    ? AdminUserType.AdminSystem
                    : user.IsAdmin
                        ? AdminUserType.CharityAdmin
                        : AdminUserType.CharityUser
                }
).FirstOrDefaultAsync();
        }





        public async Task<List<UserDto>> GetUsersByCharityIdAsync(int charityId)
        {
            return await
            (
                from user in _context.AdminUsers

                join charity in _context.Charities
                on user.CharityId equals charity.Id into charities

                from charity in charities.DefaultIfEmpty()

                where user.CharityId == charityId &&
                      !user.IsDeleted


                select new UserDto
                {
                    Id = user.Id,

                    Name = user.Name,

                    UserName = user.UserName,

                    Mobile = user.Mobile,


                    CharityId = user.CharityId,


                    CharityName =
                        charity != null
                        ? charity.Name
                        : null,


                    IsAdmin = user.IsAdmin,

                    IsRoot = user.IsRoot,

                    Sex = user.Sex,

                    CreatedAt = user.CreatedAt
                }

            ).ToListAsync();
        }




        //public async Task<AdminUser?> GetUserEntityAsync(int id)
        //{
        //    return await _context.AdminUsers
        //        .FirstOrDefaultAsync(x =>
        //            x.Id == id &&
        //            !x.IsDeleted);
        //}


        public async Task<AdminUser?> GetUserEntityAsync(int id)
        {
            return await _context.AdminUsers
                .FirstOrDefaultAsync(x => x.Id == id);
        }




        public async Task<AdminUser?> GetUserWithCharityAsync(int id)
        {
            return await _context.AdminUsers
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    !x.IsDeleted);
        }




        public async Task<Charity?> GetCharityAsync(int id)
        {
            return await _context.Charities
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    !x.IsDeleted);
        }





        public async Task UpdateUserAsync(AdminUser user)
        {
            _context.AdminUsers.Update(user);

            await SaveChangesAsync();
        }





        public async Task UpdateCharityAsync(Charity charity)
        {
            _context.Charities.Update(charity);

            await SaveChangesAsync();
        }





        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }



        public async Task DeleteAsync(AdminUser user)
        {
            user.IsDeleted = true;

            _context.AdminUsers.Update(user);

            await SaveChangesAsync();
        }

    }
}
