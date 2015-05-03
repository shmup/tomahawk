namespace Tomahawk.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class replies : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Replies",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Parent_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Messages", t => t.Parent_ID)
                .Index(t => t.Parent_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Replies", "Parent_ID", "dbo.Messages");
            DropIndex("dbo.Replies", new[] { "Parent_ID" });
            DropTable("dbo.Replies");
        }
    }
}
