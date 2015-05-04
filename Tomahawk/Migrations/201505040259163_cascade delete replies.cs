namespace Tomahawk.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cascadedeletereplies : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Replies", "Parent_ID", "dbo.Messages");
            DropIndex("dbo.Replies", new[] { "Parent_ID" });
            AlterColumn("dbo.Replies", "Parent_ID", c => c.Int(nullable: false));
            CreateIndex("dbo.Replies", "Parent_ID");
            AddForeignKey("dbo.Replies", "Parent_ID", "dbo.Messages", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Replies", "Parent_ID", "dbo.Messages");
            DropIndex("dbo.Replies", new[] { "Parent_ID" });
            AlterColumn("dbo.Replies", "Parent_ID", c => c.Int());
            CreateIndex("dbo.Replies", "Parent_ID");
            AddForeignKey("dbo.Replies", "Parent_ID", "dbo.Messages", "ID");
        }
    }
}
