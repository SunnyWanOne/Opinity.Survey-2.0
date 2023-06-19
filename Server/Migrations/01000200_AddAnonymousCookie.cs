using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Opinity.Survey.Repository;
using Opinity.Survey.Migrations.EntityBuilders;

namespace Opinity.Survey.Migrations.Migrations
{
    [DbContext(typeof(SurveyContext))]
    [Migration("Survey.01.00.02.00")]
    public class AddAnonymousCookieColumn : MultiDatabaseMigration
    {
        public AddAnonymousCookieColumn(IDatabase database) : base(database)
        {
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var surveyAnswerEntityBuilder = new OqtaneSurveyAnswerEntityBuilder(migrationBuilder, ActiveDatabase);
            surveyAnswerEntityBuilder.AddStringColumn("AnonymousCookie", 500, true, true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var surveyAnswerEntityBuilder = new OqtaneSurveyAnswerEntityBuilder(migrationBuilder, ActiveDatabase);
            surveyAnswerEntityBuilder.DropColumn("AnonymousCookie");
        }
    }
}