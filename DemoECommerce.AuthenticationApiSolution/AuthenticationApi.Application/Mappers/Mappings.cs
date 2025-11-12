using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApi.Application.Mappers
{
    public static class Mappings
    {
        /// <summary>
        /// Converts an AppUserDTO to an AppUser entity.
        /// Note: The password from the DTO is typically HASHED before setting AppUser.HashedPassword
        /// in the service layer, not directly in the mapping. For now, we'll map the fields.
        /// </summary>
        public static AppUser ToEntity(this AppUserDTO appUserDTO)
        {
            return new AppUser
            {
                Id = appUserDTO.Id,
                Name = appUserDTO.Name,
                TelephoneNumber = appUserDTO.TelephoneNumber,
                Address = appUserDTO.Address,
                Email = appUserDTO.Email,
                HashedPassword = null, // to be hashed after this process
                Role = appUserDTO.Role
            };
        }

        /// <summary>
        /// Converts an AppUser entity to a GetUserDTO.
        /// </summary>
        public static GetUserDTO ToGetDto(this AppUser appUser)
        {
            return new GetUserDTO
            (
                Id: appUser.Id,
                Name: appUser.Name!,
                TelephoneNumber: appUser.TelephoneNumber!,
                Address: appUser.Address!,
                Email: appUser.Email!,
                Role: appUser.Role!
            );
        }
    }
}
