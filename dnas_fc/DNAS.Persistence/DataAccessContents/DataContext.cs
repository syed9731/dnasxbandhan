using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DNAS.Persistence.DataAccessContents;

public partial class DataContext : DbContext
{
    protected readonly IConfiguration Configuration;
    public DataContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public virtual DbSet<Approver> Approvers { get; set; }

    public virtual DbSet<ApproverApproved> ApproverApproveds { get; set; }

    public virtual DbSet<ApproverApprovedVersion> ApproverApprovedVersions { get; set; }

    public virtual DbSet<ApproverVesion> ApproverVesions { get; set; }

    public virtual DbSet<AsignedDelegate> AsignedDelegates { get; set; }

    public virtual DbSet<AsignedDelegateApproved> AsignedDelegateApproveds { get; set; }

    public virtual DbSet<AsignedDelegateApprovedVersion> AsignedDelegateApprovedVersions { get; set; }

    public virtual DbSet<AsignedDelegateVersion> AsignedDelegateVersions { get; set; }

    public virtual DbSet<Attachment> Attachments { get; set; }

    public virtual DbSet<AttachmentApproved> AttachmentApproveds { get; set; }

    public virtual DbSet<AttachmentApprovedVersion> AttachmentApprovedVersions { get; set; }

    public virtual DbSet<AttachmentVersion> AttachmentVersions { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Configuration> Configurations { get; set; }

    public virtual DbSet<ExpenseIncurredAt> ExpenseIncurredAts { get; set; }

    public virtual DbSet<Fyi> Fyis { get; set; }

    public virtual DbSet<MailConfiguration> MailConfigurations { get; set; }

    public virtual DbSet<NatureExpensesMaster> NatureExpensesMasters { get; set; }

    public virtual DbSet<NatureOfExpense> NatureOfExpenses { get; set; }

    public virtual DbSet<Note> Notes { get; set; }

    public virtual DbSet<NoteApproved> NoteApproveds { get; set; }

    public virtual DbSet<NoteApprovedVersion> NoteApprovedVersions { get; set; }

    public virtual DbSet<NoteTracker> NoteTrackers { get; set; }

    public virtual DbSet<NoteTrackerApproved> NoteTrackerApproveds { get; set; }

    public virtual DbSet<NoteTrackerApprovedVersion> NoteTrackerApprovedVersions { get; set; }

    public virtual DbSet<NoteTrackerVersion> NoteTrackerVersions { get; set; }

    public virtual DbSet<NoteVersion> NoteVersions { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<RoleBranchMapping> RoleBranchMappings { get; set; }

    public virtual DbSet<RoleClusterMapping> RoleClusterMappings { get; set; }

    public virtual DbSet<RoleCurrencyMapping> RoleCurrencyMappings { get; set; }

    public virtual DbSet<RoleDeptMapping> RoleDeptMappings { get; set; }

    public virtual DbSet<RoleMapping> RoleMappings { get; set; }

    public virtual DbSet<RoleMaster> RoleMasters { get; set; }

    public virtual DbSet<RoleRacmapping> RoleRacmappings { get; set; }

    public virtual DbSet<RoleRegionalMapping> RoleRegionalMappings { get; set; }

    public virtual DbSet<RoleUnitMapping> RoleUnitMappings { get; set; }

    public virtual DbSet<RoleZonalMapping> RoleZonalMappings { get; set; }

    public virtual DbSet<TemplateMaster> TemplateMasters { get; set; }

    public virtual DbSet<UserMaster> UserMasters { get; set; }

    public virtual DbSet<UserTracking> UserTrackings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
=> optionsBuilder.UseSqlServer(Configuration.GetConnectionString("SQLConnection"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Approver>(entity =>
        {
            entity.ToTable("Approver");

            entity.Property(e => e.ApprovedTime).HasColumnType("datetime");
            entity.Property(e => e.ApproverType)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.AssignTime).HasColumnType("datetime");
            entity.Property(e => e.ChildAssignTime).HasColumnType("datetime");
            entity.Property(e => e.MyAssignTime).HasColumnType("datetime");
            entity.Property(e => e.SkippTime).HasColumnType("datetime");
            entity.Property(e => e.SuffixPrefix).HasColumnName("Suffix_Prefix");

            entity.HasOne(d => d.Note).WithMany(p => p.Approvers)
                .HasForeignKey(d => d.NoteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Approver_Note");

            entity.HasOne(d => d.User).WithMany(p => p.Approvers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Approver_UserMaster");
        });

        modelBuilder.Entity<ApproverApproved>(entity =>
        {
            entity.ToTable("Approver_Approved");

            entity.Property(e => e.ApproverApprovedId).HasColumnName("Approver_ApprovedId");
            entity.Property(e => e.ApprovedTime).HasColumnType("datetime");
            entity.Property(e => e.ApproverType)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.AssignTime).HasColumnType("datetime");
            entity.Property(e => e.ChildAssignTime).HasColumnType("datetime");
            entity.Property(e => e.MyAssignTime).HasColumnType("datetime");
            entity.Property(e => e.SkippTime).HasColumnType("datetime");
            entity.Property(e => e.SuffixPrefix).HasColumnName("Suffix_Prefix");
        });

        modelBuilder.Entity<ApproverApprovedVersion>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ApproverApproved_Version");

            entity.Property(e => e.ApprovedTime).HasColumnType("datetime");
            entity.Property(e => e.ApproverApprovedId).HasColumnName("Approver_ApprovedId");
            entity.Property(e => e.ApproverApprovedVersionId)
                .ValueGeneratedOnAdd()
                .HasColumnName("ApproverApproved_VersionId");
            entity.Property(e => e.ApproverType)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.AssignTime).HasColumnType("datetime");
            entity.Property(e => e.ChildAssignTime).HasColumnType("datetime");
            entity.Property(e => e.MyAssignTime).HasColumnType("datetime");
            entity.Property(e => e.NoteApprovedVersionId).HasColumnName("NoteApproved_VersionId");
            entity.Property(e => e.SkippTime).HasColumnType("datetime");
            entity.Property(e => e.SuffixPrefix).HasColumnName("Suffix_Prefix");
        });

        modelBuilder.Entity<ApproverVesion>(entity =>
        {
            entity.HasKey(e => e.ApproverIdVersionId);

            entity.ToTable("Approver_Vesion");

            entity.Property(e => e.ApproverIdVersionId).HasColumnName("ApproverId_VersionId");
            entity.Property(e => e.ApprovedTime).HasColumnType("datetime");
            entity.Property(e => e.ApproverType)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.AssignTime).HasColumnType("datetime");
            entity.Property(e => e.ChildAssignTime).HasColumnType("datetime");
            entity.Property(e => e.MyAssignTime).HasColumnType("datetime");
            entity.Property(e => e.NoteVersionId).HasColumnName("NoteVersion_Id");
            entity.Property(e => e.SkippTime).HasColumnType("datetime");
            entity.Property(e => e.SuffixPrefix).HasColumnName("Suffix_Prefix");
        });

        modelBuilder.Entity<AsignedDelegate>(entity =>
        {
            entity.HasKey(e => e.DelegateId);

            entity.ToTable("AsignedDelegate");

            entity.Property(e => e.ApproverId).HasColumnName("ApproverID");
            entity.Property(e => e.AssignTime).HasColumnType("datetime");
            entity.Property(e => e.DeligatedUserId).HasColumnName("Deligated_UserId");

            entity.HasOne(d => d.Approver).WithMany(p => p.AsignedDelegates)
                .HasForeignKey(d => d.ApproverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AsignedDelegate_Approver");

            entity.HasOne(d => d.DeligatedUser).WithMany(p => p.AsignedDelegates)
                .HasForeignKey(d => d.DeligatedUserId)
                .HasConstraintName("FK_AsignedDelegate_UserMaster");
        });

        modelBuilder.Entity<AsignedDelegateApproved>(entity =>
        {
            entity.HasKey(e => e.DelegateApprovedId);

            entity.ToTable("AsignedDelegate_Approved");

            entity.Property(e => e.DelegateApprovedId).HasColumnName("Delegate_ApprovedId");
            entity.Property(e => e.ApproverId).HasColumnName("ApproverID");
            entity.Property(e => e.AssignTime).HasColumnType("datetime");
            entity.Property(e => e.DeligatedUserId).HasColumnName("Deligated_UserId");
        });

        modelBuilder.Entity<AsignedDelegateApprovedVersion>(entity =>
        {
            entity.HasKey(e => e.DelegateApprovedVersionId);

            entity.ToTable("AsignedDelegateApproved_Version");

            entity.Property(e => e.DelegateApprovedVersionId).HasColumnName("DelegateApproved_VersionId");
            entity.Property(e => e.ApproverId).HasColumnName("ApproverID");
            entity.Property(e => e.AssignTime).HasColumnType("datetime");
            entity.Property(e => e.DelegateApprovedId).HasColumnName("Delegate_ApprovedId");
            entity.Property(e => e.DeligatedUserId).HasColumnName("Deligated_UserId");
            entity.Property(e => e.NoteApprovedVersionId).HasColumnName("NoteApproved_VersionId");
        });

        modelBuilder.Entity<AsignedDelegateVersion>(entity =>
        {
            entity.HasKey(e => e.DelegateIdVersionId);

            entity.ToTable("AsignedDelegate_Version");

            entity.Property(e => e.DelegateIdVersionId).HasColumnName("DelegateId_VersionId");
            entity.Property(e => e.ApproverId).HasColumnName("ApproverID");
            entity.Property(e => e.AssignTime).HasColumnType("datetime");
            entity.Property(e => e.DeligatedUserId).HasColumnName("Deligated_UserId");
            entity.Property(e => e.NoteVersionId).HasColumnName("NoteVersion_Id");
        });

        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.ToTable("Attachment");

            entity.Property(e => e.AttachmentPath)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.DocumentName)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.Note).WithMany(p => p.Attachments)
                .HasForeignKey(d => d.NoteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attachment_Note");
        });

        modelBuilder.Entity<AttachmentApproved>(entity =>
        {
            entity.ToTable("Attachment_Approved");

            entity.Property(e => e.AttachmentApprovedId).HasColumnName("Attachment_ApprovedId");
            entity.Property(e => e.AttachmentPath)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.DocumentName)
                .HasMaxLength(500)
                .IsUnicode(false);
        });

        modelBuilder.Entity<AttachmentApprovedVersion>(entity =>
        {
            entity.ToTable("AttachmentApproved_Version");

            entity.Property(e => e.AttachmentApprovedVersionId).HasColumnName("AttachmentApproved_VersionId");
            entity.Property(e => e.AttachmentApprovedId).HasColumnName("Attachment_ApprovedId");
            entity.Property(e => e.AttachmentPath)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.DocumentName)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.NoteApprovedVersionId).HasColumnName("NoteApproved_VersionId");
        });

        modelBuilder.Entity<AttachmentVersion>(entity =>
        {
            entity.ToTable("Attachment_Version");

            entity.Property(e => e.AttachmentVersionId).HasColumnName("Attachment_VersionId");
            entity.Property(e => e.AttachmentPath)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.DocumentName)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.NoteVersionId).HasColumnName("NoteVersion_Id");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");

            entity.Property(e => e.CategoryName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Configuration>(entity =>
        {
            entity.ToTable("Configuration");

            entity.Property(e => e.ConfigurationFor)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ConfigurationKey)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ConfigurationValue)
                .HasMaxLength(500)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ExpenseIncurredAt>(entity =>
        {
            entity.HasKey(e => e.ExpenseIncurredAtId).HasName("PK_SubCategory");

            entity.ToTable("ExpenseIncurredAt");

            entity.Property(e => e.ExpenseIncurredAtName)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Category).WithMany(p => p.ExpenseIncurredAts)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_SubCategory_Category");
        });

        modelBuilder.Entity<Fyi>(entity =>
        {
            entity.ToTable("FYI");

            entity.Property(e => e.Fyiid).HasColumnName("FYIId");
            entity.Property(e => e.TaggedTime).HasColumnType("datetime");

            entity.HasOne(d => d.Note).WithMany(p => p.Fyis)
                .HasForeignKey(d => d.NoteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FYI_Note");

            entity.HasOne(d => d.ToWhomeNavigation).WithMany(p => p.FyiToWhomeNavigations)
                .HasForeignKey(d => d.ToWhome)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FYI_UserMaster1");

            entity.HasOne(d => d.WhoTaggedNavigation).WithMany(p => p.FyiWhoTaggedNavigations)
                .HasForeignKey(d => d.WhoTagged)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FYI_UserMaster");
        });

        modelBuilder.Entity<MailConfiguration>(entity =>
        {
            entity.ToTable("MailConfiguration");

            entity.Property(e => e.MailBody).IsUnicode(false);
            entity.Property(e => e.MailKey)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MailSubject)
                .HasMaxLength(250)
                .IsUnicode(false);
        });

        modelBuilder.Entity<NatureExpensesMaster>(entity =>
        {
            entity.HasKey(e => e.NatureExpensesId);

            entity.ToTable("NatureExpensesMaster");

            entity.Property(e => e.NatureOfExpenseCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NatureOfExpensesName)
                .HasMaxLength(5000)
                .IsUnicode(false);
        });

        modelBuilder.Entity<NatureOfExpense>(entity =>
        {
            entity.HasKey(e => e.NatureOfExpensesId);

            entity.Property(e => e.MaxAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MinAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Uom)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("UOM");
        });

        modelBuilder.Entity<Note>(entity =>
        {
            entity.ToTable("Note", tb => tb.HasTrigger("GenerateNoteUUID_Trg"));

            entity.HasIndex(e => e.NoteId, "IX_Note").IsUnique();

            entity.Property(e => e.CapitalExpenditure).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatorDepartment)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.DateOfCreation)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NoteState)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.NoteStatus)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.NoteTitle)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.NoteUid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NoteUID");
            entity.Property(e => e.OperationalExpenditure).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.WithdrawDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<NoteApproved>(entity =>
        {
            entity.ToTable("Note_Approved", tb => tb.HasTrigger("NoteApproved_Vesion_Trg"));

            entity.Property(e => e.NoteApprovedId).HasColumnName("Note_ApprovedId");
            entity.Property(e => e.CapitalExpenditure).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatorDepartment)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.DateOfCreation).HasColumnType("datetime");
            entity.Property(e => e.NoteState)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.NoteStatus)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.NoteTitle)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.NoteUid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NoteUID");
            entity.Property(e => e.OperationalExpenditure).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.WithdrawDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<NoteApprovedVersion>(entity =>
        {
            entity.ToTable("NoteApproved_Version");

            entity.Property(e => e.NoteApprovedVersionId).HasColumnName("NoteApproved_VersionId");
            entity.Property(e => e.CapitalExpenditure).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatorDepartment)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.DateOfCreation).HasColumnType("datetime");
            entity.Property(e => e.NoteApprovedId).HasColumnName("Note_ApprovedId");
            entity.Property(e => e.NoteState)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.NoteStatus)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.NoteTitle)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.NoteUid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NoteUID");
            entity.Property(e => e.OperationalExpenditure).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.WithdrawDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<NoteTracker>(entity =>
        {
            entity.HasKey(e => e.NoteTackerId);

            entity.ToTable("NoteTracker");

            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.CommentTime).HasColumnType("datetime");
            entity.Property(e => e.NoteStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<NoteTrackerApproved>(entity =>
        {
            entity.HasKey(e => e.NoteTackerApprovedId);

            entity.ToTable("NoteTracker_Approved");

            entity.Property(e => e.NoteTackerApprovedId).HasColumnName("NoteTacker_ApprovedId");
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.CommentTime).HasColumnType("datetime");
            entity.Property(e => e.NoteStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<NoteTrackerApprovedVersion>(entity =>
        {
            entity.HasKey(e => e.NoteTackerApprovedVersionId);

            entity.ToTable("NoteTrackerApproved_Version");

            entity.Property(e => e.NoteTackerApprovedVersionId).HasColumnName("NoteTackerApproved_VersionId");
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.CommentTime).HasColumnType("datetime");
            entity.Property(e => e.NoteApprovedVersionId).HasColumnName("NoteApproved_VersionId");
            entity.Property(e => e.NoteStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NoteTackerApprovedId).HasColumnName("NoteTacker_ApprovedId");
        });

        modelBuilder.Entity<NoteTrackerVersion>(entity =>
        {
            entity.HasKey(e => e.NoteTackerVersionId);

            entity.ToTable("NoteTracker_Version");

            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.CommentTime).HasColumnType("datetime");
            entity.Property(e => e.NoteStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NoteVersionId).HasColumnName("Note_VersionId");

            entity.HasOne(d => d.NoteVersion).WithMany(p => p.NoteTrackerVersions)
                .HasForeignKey(d => d.NoteVersionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NoteTracker_Version_Note_Version");
        });

        modelBuilder.Entity<NoteVersion>(entity =>
        {
            entity.ToTable("Note_Version");

            entity.Property(e => e.NoteVersionId).HasColumnName("Note_VersionId");
            entity.Property(e => e.CapitalExpenditure).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatorDepartment)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.DateOfCreation).HasColumnType("datetime");
            entity.Property(e => e.NoteState)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.NoteStatus)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.NoteTitle)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.NoteUid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NoteUID");
            entity.Property(e => e.OperationalExpenditure).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.WithdrawDate).HasColumnType("datetime");

            entity.HasOne(d => d.Note).WithMany(p => p.NoteVersions)
                .HasForeignKey(d => d.NoteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Note_Version_Note");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notification");

            entity.Property(e => e.Action)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Heading)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Message).IsUnicode(false);
            entity.Property(e => e.NotificationTime).HasColumnType("datetime");

            entity.HasOne(d => d.Note).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.NoteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notification_Note");

            entity.HasOne(d => d.ReceiverUser).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.ReceiverUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notification_Notification");
        });

        modelBuilder.Entity<RoleBranchMapping>(entity =>
        {
            entity.HasKey(e => e.MappingId);

            entity.ToTable("RoleBranchMapping");

            entity.Property(e => e.MappingId).HasColumnName("MappingID");
            entity.Property(e => e.BranchName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
        });

        modelBuilder.Entity<RoleClusterMapping>(entity =>
        {
            entity.HasKey(e => e.MappingId);

            entity.ToTable("RoleClusterMapping");

            entity.Property(e => e.MappingId).HasColumnName("MappingID");
            entity.Property(e => e.ClusterName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
        });

        modelBuilder.Entity<RoleCurrencyMapping>(entity =>
        {
            entity.HasKey(e => e.MappingId);

            entity.ToTable("RoleCurrencyMapping");

            entity.Property(e => e.MappingId).HasColumnName("MappingID");
            entity.Property(e => e.BranchName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
        });

        modelBuilder.Entity<RoleDeptMapping>(entity =>
        {
            entity.HasKey(e => e.MappingId);

            entity.ToTable("RoleDeptMapping");

            entity.Property(e => e.MappingId).HasColumnName("MappingID");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
        });

        modelBuilder.Entity<RoleMapping>(entity =>
        {
            entity.HasKey(e => e.MappingId);

            entity.ToTable("RoleMapping");

            entity.Property(e => e.MappingId).HasColumnName("MappingID");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
        });

        modelBuilder.Entity<RoleMaster>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK_Department");

            entity.ToTable("RoleMaster");

            entity.HasIndex(e => e.RoleName, "IX_Designation").IsUnique();

            entity.Property(e => e.RoleName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<RoleRacmapping>(entity =>
        {
            entity.HasKey(e => e.MappingId);

            entity.ToTable("RoleRACMapping");

            entity.Property(e => e.MappingId).HasColumnName("MappingID");
            entity.Property(e => e.BranchName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
        });

        modelBuilder.Entity<RoleRegionalMapping>(entity =>
        {
            entity.HasKey(e => e.MappingId);

            entity.ToTable("RoleRegionalMapping");

            entity.Property(e => e.MappingId).HasColumnName("MappingID");
            entity.Property(e => e.RegionName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
        });

        modelBuilder.Entity<RoleUnitMapping>(entity =>
        {
            entity.HasKey(e => e.MappingId);

            entity.ToTable("RoleUnitMapping");

            entity.Property(e => e.MappingId).HasColumnName("MappingID");
            entity.Property(e => e.BranchName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
        });

        modelBuilder.Entity<RoleZonalMapping>(entity =>
        {
            entity.HasKey(e => e.MappingId);

            entity.ToTable("RoleZonalMapping");

            entity.Property(e => e.MappingId).HasColumnName("MappingID");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.ZonalName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TemplateMaster>(entity =>
        {
            entity.HasKey(e => e.TemplateId);

            entity.ToTable("TemplateMaster");

            entity.Property(e => e.DateOfCreation).HasColumnType("datetime");
            entity.Property(e => e.TemplateName)
                .HasMaxLength(250)
                .IsUnicode(false);

            entity.HasOne(d => d.Category).WithMany(p => p.TemplateMasters)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TemplateMaster_Category");

            entity.HasOne(d => d.User).WithMany(p => p.TemplateMasters)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TemplateMaster_UserMaster");
        });

        modelBuilder.Entity<UserMaster>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("UserMaster");

            entity.Property(e => e.BranchName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Cluster)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Department)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Designation)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FunctionalReportingTo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Functional_Reporting_To");
            entity.Property(e => e.Grade)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LastLoginTime).HasColumnType("datetime");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastPassRecoveryTime).HasColumnType("datetime");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password).IsUnicode(false);
            entity.Property(e => e.Region)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ReportingTo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Reporting_To");
            entity.Property(e => e.Role)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Seniority)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.UserEmpId)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Zone)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UserTracking>(entity =>
        {
            entity.ToTable("UserTracking");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.LastLoginTime).HasColumnType("datetime");
            entity.Property(e => e.SessionId)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
