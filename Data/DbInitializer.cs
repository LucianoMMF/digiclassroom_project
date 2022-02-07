using DigiClassroom.Data;
using DigiClassroom.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoFinal.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context
            , UserManager<AppUser> userManager)
        {
            context.Database.EnsureCreated();

            // Look for any utilizadores.

            if (context.ClassroomUsers.Any())
            {
                return;   // DB has been seeded
            }

            var adminAppUserId = await CreateAdmin(userManager);
            CreateClassrooms(context, userManager, adminAppUserId);
            var students = await CreateStudents(context, userManager);
            var mentors = await CreateMentors(context, userManager);
            CreatePublishes(context, userManager, students, mentors);
            CreateComments(context, userManager, students, mentors);
        }

        private static async Task<string> CreateAdmin(UserManager<AppUser> userManager)
        {
            var user = new AppUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true,
            };

            string adminAppUserId = "";

            var result = await userManager.CreateAsync(user, "Password1!");
            if (result.Succeeded)
            {
                adminAppUserId = user.Id;
            }

            return adminAppUserId;
        }

        private static void CreateClassrooms(ApplicationDbContext context, UserManager<AppUser> userManager, string adminAppUserId)
        {
            var classrooms = new Classroom[]
            {
            new Classroom{ID=1, AppUserID=adminAppUserId, title="Classroom 1", description="Desc classroom 1", time_created=System.DateTime.Now.AddDays(-20)},
            new Classroom{ID=2, AppUserID=adminAppUserId, title="Classroom 2", description="Desc classroom 2", time_created=System.DateTime.Now.AddDays(-20)},
            new Classroom{ID=3, AppUserID=adminAppUserId, title="Classroom 3", description="Desc classroom 3", time_created=System.DateTime.Now.AddDays(-10)},
            new Classroom{ID=4, AppUserID=adminAppUserId, title="Classroom 4", description="Desc classroom 4", time_created=System.DateTime.Now.AddDays(-2)},
            };
            foreach (Classroom s in classrooms)
            {
                context.Classrooms.Add(s);
            }
            context.SaveChanges();
        }

        private static async Task<ClassroomUser[]> CreateStudents(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            var names = new[] { "Rui", "Rodrigo", "Fabio", "Maria", "Mafalda", "Sergio", "Paula", "Vitoria", "Mariana", "Andre" };
            var students = new List<ClassroomUser>();
            var classroomId = 1;
            foreach (var name in names)
            {

                var email = name.ToLower().Replace(" ", ".") + "@gmail.com";
                var user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                };

                
                var result = await userManager.CreateAsync(user, "Password1!");

                if (result.Succeeded)
                {
                    var classroomUser = new ClassroomUser { Role = "Student" };
                    classroomUser.AppUserId = user.Id;
                    classroomUser.ClassroomId = classroomId++;
                    if (classroomId == 4) classroomId = 1; // reset if we reach the last classroom
                    context.ClassroomUsers.Add(classroomUser);

                    students.Add(classroomUser);
                }

            }
            context.SaveChanges();

            return students.ToArray();
        }

        private static async Task<ClassroomUser[]> CreateMentors(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            var names = new[] { "Sara", "Catarina", "Ricardo", "Jorge", "John" };
            var mentores = new List<ClassroomUser>();
            var classroomId = 1;
            foreach (var name in names)
            {

                var email = name.ToLower().Replace(" ", ".") + "@gmail.com";
                var user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                };


                var result = await userManager.CreateAsync(user, "Password1!");

                if (result.Succeeded)
                {
                    var classroomUser = new ClassroomUser { Role = "Mentor" };
                    classroomUser.AppUserId = user.Id;
                    classroomUser.ClassroomId = classroomId++;
                    if (classroomId == 4) classroomId = 1; // reset if we reach the last classroom
                    context.ClassroomUsers.Add(classroomUser);

                    mentores.Add(classroomUser);
                }

            }
            context.SaveChanges();

            return mentores.ToArray();
        }

        private static void CreatePublishes(ApplicationDbContext context, UserManager<AppUser> userManager, ClassroomUser[] alunos, ClassroomUser[] professores)
        {
            var publishes = new BlackBoard[]
            {
            new BlackBoard{content="Publicaçao 1",ClassroomId=1,AppUserId=professores[0].AppUserId},
            new BlackBoard{content="Publicaçao 2",ClassroomId=2,AppUserId=professores[1].AppUserId},
            new BlackBoard{content="Publicaçao 3",ClassroomId=3,AppUserId=professores[2].AppUserId},
            };
            foreach (BlackBoard s in publishes)
            {
                context.BlackBoards.Add(s);
            }
            context.SaveChanges();
        }

        private static void CreateComments(ApplicationDbContext context, UserManager<AppUser> userManager, ClassroomUser[] alunos, ClassroomUser[] professores)
        {
            var comments = new Comment[]
            {
            new Comment{Content="Comentário 1, a publicacao 1", BlackBoardId=1,AppUserId=alunos[0].AppUserId},
            new Comment{Content="Comentário 2, a publicacao 1", BlackBoardId=1,AppUserId=professores[0].AppUserId},
            new Comment{Content="Comentário 1, a publicacao 2", BlackBoardId=2,AppUserId=alunos[1].AppUserId},
            new Comment{Content="Comentário 1, a publicacao 3", BlackBoardId=3,AppUserId=alunos[1].AppUserId},
            new Comment{Content="Comentário 2, a publicacao 3", BlackBoardId=3,AppUserId=professores[2].AppUserId},
            new Comment{Content="Comentário 3, a publicacao 3", BlackBoardId=3,AppUserId=alunos[3].AppUserId},
            };
            foreach (Comment s in comments)
            {
                context.Comments.Add(s);
            }
            context.SaveChanges();
        }

    }
}
