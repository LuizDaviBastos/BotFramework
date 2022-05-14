using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotScheduler
{
    public class ChatContext : DbContext
    {
        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //   => options.UseSqlServer("Server=tcp:dss-fernando.database.windows.net,1433;Initial Catalog=telehealth;Persist Security Info=False;User ID=dss-fernando-admin;Password=wEsq45mqDZEHBBkz;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

        
        public ChatContext(DbContextOptions<ChatContext> options) : base(options)
        {
        }

        public DbSet<Model.Chat> Chats { get; set; }
        public DbSet<Model.ChatMessage> ChatMessages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Model.Chat>().ToTable("Chat");
            modelBuilder.Entity<Model.ChatMessage>().ToTable("ChatMessage");
        }
    }
}
