﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinterProjectAPIV3.DataTransferObjects;
using WinterProjectAPIV3.Models;

namespace WinterProjectAPIV3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserGroupController : ControllerBase
    {
        private readonly PaymentApidbContext context;
        public UserGroupController(PaymentApidbContext context)
        {
            this.context = context;
        }

        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<List<ShareUser>>> GetUsers()
        {
            var users = await context.ShareUsers.ToListAsync();
            if (users == null)
            {
                return NotFound();
            }
            return Ok(users);
        }

        [HttpGet("GetUserByID/{ID}")]
        public async Task<ActionResult<ShareUser>> GetUserOnID(int ID)
        {
            var SearchedUser = context.ShareUsers.Find(ID);
            if (SearchedUser == null)
            {
                return NotFound();
            }
            return Ok(SearchedUser);
        }

        [HttpPost("CreateUser")]
        public async Task<ActionResult<List<ShareUser>>> CreateShareUser(CreateShareUserDto request)
        {
            //Create the user to insert
            ShareUser UserToInsert = new ShareUser
            {
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = request.Password
            };

            //Insert the user
            context.ShareUsers.Add(UserToInsert);
            await context.SaveChangesAsync();

            return await GetUsers();
        }

        //Update details of a user
        [HttpPut("UpdateUser")]
        public async Task<ActionResult<ShareUser>> UpdateUserDetails(ShareUser request)
        {
            ShareUser RecordToChange = context.ShareUsers.Find(request.UserId);
            if (RecordToChange != null)
            {
                RecordToChange.UserName = request.UserName;
                RecordToChange.PhoneNumber = request.PhoneNumber;
                RecordToChange.FirstName = request.FirstName;
                RecordToChange.LastName = request.LastName;
                RecordToChange.Email = request.Email;
                RecordToChange.Password = request.Password;
            }
            else
            {
                return NotFound();
            }
            await context.SaveChangesAsync();
            return RecordToChange;
        }

        [HttpGet("GetAllGroups")]
        public async Task<ActionResult<List<ShareGroup>>> GetAllGroups()
        {
            List<ShareGroup> AllGroupsList = await context.ShareGroups.ToListAsync();
            if (AllGroupsList == null)
            {
                return NotFound();
            }
            return Ok(AllGroupsList);
        }

        [HttpPost("CreateGroup")]
        public async Task<ActionResult<List<ShareGroup>>> CreateGroup(CreateShareGroupDto request)
        {
            //Create a ShareGroup entry
            ShareGroup GroupToInsert = new ShareGroup
            {
                Name = request.Name,
                Description = request.Description,
                HasConcluded = request.HasConcluded
            };
            //Insert the Group
            context.ShareGroups.AddAsync(GroupToInsert);
            await context.SaveChangesAsync();

            //Get the ID of inserted ShareGroup
            int NewlyCreatedID = GroupToInsert.GroupId;

            //Create a UserGroup entry
            UserGroup UserGroupToInsert = new UserGroup
            {
                UserId = request.UserID,
                GroupId = NewlyCreatedID,
                IsOwner = true,
            };

            context.UserGroups.AddAsync(UserGroupToInsert);
            await context.SaveChangesAsync();

            return await GetAllGroups();
        }

        [HttpPost("JoinExistingGroup")]
        public async Task<ActionResult<UserGroup>> JoinGroup(JoinGroupDto request)
        {
            //Insert into UserGroup table
            ShareGroup ExistingGroup = context.ShareGroups.Find(request.GroupID);
            
            //Make sure you the user isn't already in the group
            var ExistingUserGroupQuery = from usergroup in context.UserGroups
                where usergroup.UserId == request.UserID && usergroup.GroupId == request.GroupID select new
                {
                    usergroup.UserGroupId,
                    usergroup.UserId,
                    usergroup.GroupId
                };

            List<JoinGroupDto> ExistingUserGroupsList = new List<JoinGroupDto>();

            foreach (var record in ExistingUserGroupQuery)
            {
                ExistingUserGroupsList.Add(new JoinGroupDto
                {
                    UserID = record.UserId,
                    GroupID = record.GroupId
                });
            }

            
            if (ExistingUserGroupsList.Count > 0)
            {
                return Conflict();
            }
            if (ExistingGroup == null)
            {
                return NotFound();
            }
            UserGroup UserGroupToInsert = new UserGroup
            {
                UserId = request.UserID,
                GroupId = request.GroupID,
                IsOwner = false
            };

            context.UserGroups.Add(UserGroupToInsert);
            await context.SaveChangesAsync();

            //Query the UserGroup for that GroupID

            var UserGroups = await context.UserGroups
                .Where(UserGroup => UserGroup.GroupId == request.GroupID)
                .Include(UserGroup => UserGroup.User)
                .ToListAsync();
            return Ok(UserGroups);
        }

        [HttpPut("ConcludeGroup")]
        public async Task<ActionResult<ShareGroup>> ConcludeGroup(int GroupID)
        {
            var GroupToEnd = context.ShareGroups.Find(GroupID);
            if (GroupToEnd != null)
            {
                GroupToEnd.HasConcluded = true;
            }
            else
            {
                return NotFound();
            }

            await context.SaveChangesAsync();
            ShareGroup ConcludedGroup = await context.ShareGroups.FindAsync(GroupID);
            return Ok(ConcludedGroup);
        }

        [HttpGet("GetAllExpenses")]
        public async Task<ActionResult<List<Expense>>> GetAllExpenses()
        {
            return Ok(await context.Expenses.ToListAsync());
        }

        [HttpGet("GetAllExpensesOnGroupID")]
        public async Task<ActionResult<List<GetAllExpensesDto>>> getAllExpensesOnGroupID(int GroupID)
        {
            var query = from expense in context.Expenses
                        join usergroup in context.UserGroups on expense.UserGroupId equals usergroup.UserGroupId
                        join sharegroup in context.ShareGroups on usergroup.GroupId equals sharegroup.GroupId
                        join shareuser in context.ShareUsers on usergroup.UserId equals shareuser.UserId
                        where usergroup.GroupId == GroupID
                        select new
                        {
                            expense.ExpenseId,
                            expense.Amount,
                            usergroup.UserId,
                            usergroup.GroupId,
                            sharegroup.Name,
                            shareuser.UserName,
                            shareuser.PhoneNumber,
                            shareuser.FirstName,
                            shareuser.LastName,
                            shareuser.Email
                        };
            List<GetAllExpensesDto> QueriedList = new List<GetAllExpensesDto>();
            foreach (var record in query)
            {
                QueriedList.Add(new GetAllExpensesDto
                {
                    ExpenseId = record.ExpenseId,
                    Amount = record.Amount,
                    UserId = record.UserId,
                    GroupId = record.GroupId,
                    Name = record.Name,
                    UserName = record.UserName,
                    PhoneNumber = record.PhoneNumber,
                    FirstName = record.FirstName,
                    LastName = record.LastName,
                    Email = record.Email
                });
            }

            if (QueriedList.Count == 0)
            {
                return NotFound();
            }
            else
            {
                return Ok(QueriedList);
            }
        }

        [HttpPost("InsertExpenditure")]
        public async Task<ActionResult<List<GetAllExpensesDto>>> InsertNewExpenditure(CreateExpenditureDto request)
        {
            //Get the UserGroupID
            var query = from usergroup in context.UserGroups
                where usergroup.GroupId == request.GroupID &&
                      usergroup.UserId == request.UserID
                select new
                {
                    usergroup.UserGroupId
                };
            
            
            int UserGroupID = -1;
            foreach (var usergroup in query)
            {
                UserGroupID = usergroup.UserGroupId;
            }
            //If the Usergroup for that expenditure doesn't exist
            if (UserGroupID == -1)
            {
                return NotFound(getAllExpensesOnGroupID(request.GroupID));
                //return BadRequest("Bad request");
            }

            //Insert into Expenses using the amount and UserGroupID

            Expense ExpenseToInsert = new Expense
            {
                UserGroupId = UserGroupID,
                Amount = request.Amount
            };

            context.Expenses.Add(ExpenseToInsert);
            await context.SaveChangesAsync();
            
            return await getAllExpensesOnGroupID(request.GroupID);
        }

        [HttpDelete("DeleteExpenditure/{ExpenseID}")]
        public async Task<ActionResult<List<GetAllExpensesDto>>> DeleteExpenditureOnID(int ExpenseID)
        {
            //Get the UserGroupID of this expenseID
            //Get the GroupID associate with the UserGroupID
            var GetUserGroupIDQuery = from expense in context.Expenses
                join usergroup in context.UserGroups on expense.UserGroupId equals usergroup.UserGroupId
                where expense.ExpenseId == ExpenseID
                select new
                {
                    expense.UserGroupId,
                    usergroup.UserId,
                    usergroup.GroupId
                };
            int? UserGroupID = -1;
            int? UserID = -1;
            int? GroupID = -1;
            foreach (var record in GetUserGroupIDQuery)
            {
                UserGroupID = record.UserGroupId;
                UserID = record.UserId;
                GroupID = record.GroupId;
            }
            
            //Delete the expenseID from the table
            await context.Expenses.Where(x => x.ExpenseId == ExpenseID).ExecuteDeleteAsync();
            await context.SaveChangesAsync();

            //Get all expenses for this GroupID
            return await getAllExpensesOnGroupID((int)GroupID);
        }

        public async void CalculateIndividualSharesInGroup()
        {
            
        }


    }
}