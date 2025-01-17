﻿using Business.Abstract;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Authorization;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Autofac.Validation;
using Core.Business;
using Core.Entities.Concrete;
using Core.Entities.DTOs;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using System.Collections.Generic;

namespace Business.Concrete
{
    public class UserManager : BusinessService, IUserService
    {
        private readonly IUserDal _userDal;

        public UserManager(IUserDal userDal)
        {
            _userDal = userDal;
        }

        [TransactionScopeAspect]
        [ValidationAspect(typeof(UserValidator))]
        [CacheRemoveAspect("IUserService.Get")]
        public IResult Add(User entity)
        {
            var result = BusinessRules.Run();

            if (!result.Success) return result;

            _userDal.Add(entity);
            return new SuccessResult();
        }

        [LoginRequired]
        [SecuredOperation("", "entity.Id")]
        [TransactionScopeAspect]
        [CacheRemoveAspect("IUserService.Get")]
        public IResult Delete(DeleteModel entity)
        {
            var result = BusinessRules.Run();

            if (!result.Success) return result;

            var entityToDelete = GetById(entity.ID).Data;
            _userDal.Delete(entityToDelete);
            return new SuccessResult();
        }

        [LoginRequired]
        [SecuredOperation("", "entity.Id")]
        [TransactionScopeAspect]
        [ValidationAspect(typeof(UserValidator))]
        [CacheRemoveAspect("IUserService.Get")]
        public IResult Update(User entity)
        {
            var result = BusinessRules.Run();

            if (!result.Success) return result;

            _userDal.Update(entity);
            return new SuccessResult();
        }

        [LoginRequired]
        [SecuredOperation("", "id")]
        public IDataResult<User> GetById(int id)
        {
            return new SuccessDataResult<User>(_userDal.Get(u => u.UserId == id));
        }

        [SecuredOperation("Admin")]
        [CacheAspect(10)]
        public IDataResult<List<User>> GetAll()
        {
            return new SuccessDataResult<List<User>>(_userDal.GetAll());
        }

        [SecuredOperation("Admin")]
        public IDataResult<List<User>> GetByStatus(bool status)
        {
            return new SuccessDataResult<List<User>>(_userDal.GetAll(u => u.Status == status));
        }

        [LoginRequired]
        [SecuredOperation("", "email")]
        public IDataResult<User> GetByEmail(string email)
        {
            return new SuccessDataResult<User>(_userDal.Get(u => u.Email == email));
        }

        public IDataResult<User> GetByEmailForAuth(string email)
        {
            return new SuccessDataResult<User>(_userDal.Get(u => u.Email == email));
        }
        
        public IDataResult<List<OperationClaim>> GetClaims(User user)
        {
            return new SuccessDataResult<List<OperationClaim>>(_userDal.GetClaims(user));
        }

        [TransactionScopeAspect]
        public IDataResult<User> AddWithId(User entity)
        {
            var result = BusinessRules.Run(CheckIfUserAlreadyExists(entity.Email));

            if (!result.Success) return new ErrorDataResult<User>(result.Message);
            _userDal.Add(entity);
            return new SuccessDataResult<User>(entity);
        }

        [TransactionScopeAspect]
        public IResult UpdateForAuth(User entity)
        {
            var result = BusinessRules.Run();

            if (!result.Success) return result;

            _userDal.Update(entity);
            return new SuccessResult();
        }

        public IDataResult<User> GetByIdForAuth(int id)
        {
            return new SuccessDataResult<User>(_userDal.Get(u => u.UserId == id));
        }

        private IResult CheckIfUserAlreadyExists(string email)
        {
            var result = GetByEmail(email).Data;

            if (result != null) return new ErrorResult();


            return new SuccessResult();
        }
    }
}