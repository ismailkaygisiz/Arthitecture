﻿using System.Collections.Generic;
using Business.Abstract;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Authorization;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Autofac.Validation;
using Core.Business;
using Core.Entities.Concrete;
using Core.Entities.DTOs;
using Core.Utilities.Constants;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;

namespace Business.Concrete
{
    public class UserManager : IUserService
    {
        private IUserDal _userDal;

        public UserManager(IUserDal userDal)
        {
            _userDal = userDal;
        }

        [TransactionScopeAspect]
        [ValidationAspect(typeof(UserValidator))]
        public IResult Add(User entity)
        {
            IResult result = BusinessRules.Run();

            if (result != null)
            {
                return result;
            }

            _userDal.Add(entity);
            return new SuccessResult();
        }

        [SecuredOperation("User", "entity.Id")]
        [TransactionScopeAspect]
        public IResult Delete(User entity)
        {
            IResult result = BusinessRules.Run();

            if (result != null)
            {
                return result;
            }

            var entityToDelete = GetById(entity.Id).Data;

            _userDal.Delete(entityToDelete);
            return new SuccessResult();
        }

        [SecuredOperation("User", "entity.Id")]
        [TransactionScopeAspect]
        [ValidationAspect(typeof(UserValidator))]
        public IResult Update(User entity)
        {
            IResult result = BusinessRules.Run();

            if (result != null)
            {
                return result;
            }

            _userDal.Update(entity);
            return new SuccessResult();
        }

        [SecuredOperation("User", "id")]
        public IDataResult<User> GetById(int id)
        {
            return new SuccessDataResult<User>(_userDal.Get(u => u.Id == id));
        }

        [SecuredOperation("Admin")]
        public IDataResult<List<User>> GetAll()
        {
            return new SuccessDataResult<List<User>>(_userDal.GetAll());
        }

        [SecuredOperation("Admin")]
        public IDataResult<List<User>> GetByFirstName(string firstName)
        {
            return new SuccessDataResult<List<User>>(_userDal.GetAll(u => u.FirstName == firstName));
        }

        [SecuredOperation("Admin")]
        public IDataResult<List<User>> GetByLastName(string lastName)
        {
            return new SuccessDataResult<List<User>>(_userDal.GetAll(u => u.LastName == lastName));
        }

        [SecuredOperation("Admin")]
        public IDataResult<List<User>> GetByStatus(bool status)
        {
            return new SuccessDataResult<List<User>>(_userDal.GetAll(u => u.Status == status));
        }

        [SecuredOperation("User", "email")]
        public IDataResult<User> GetByEmail(string email)
        {
            return new SuccessDataResult<User>(_userDal.Get(u => u.Email == email));
        }

        public IDataResult<User> GetByEmailForAuth(string email)
        {
            return new SuccessDataResult<User>(_userDal.Get(u=>u.Email == email));
        }

        [SecuredOperation("Admin")]
        public IDataResult<UserOperationClaimDetailDto> GetUserOperationClaims(int userId)
        {
            return new SuccessDataResult<UserOperationClaimDetailDto>(_userDal.GetUserOperationClaimsDetails(userId));
        }

        public IDataResult<List<OperationClaim>> GetClaims(User user)
        {
            return new SuccessDataResult<List<OperationClaim>>(_userDal.GetClaims(user));
        }
    }
}
