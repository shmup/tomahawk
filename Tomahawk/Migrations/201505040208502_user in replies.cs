namespace Tomahawk.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userinreplies : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Replies", "User_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Replies", "User_Id");
            AddForeignKey("dbo.Replies", "User_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Replies", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Replies", new[] { "User_Id" });
            DropColumn("dbo.Replies", "User_Id");
        }
    }
}
