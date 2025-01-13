using Microsoft.EntityFrameworkCore;
using Backend.DataBase.Data.Models;

namespace Backend.DataBase.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    // DbSety reprezentujące tabele w bazie danych
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Reservation> Reservations { get; set; } = null!;
    public DbSet<Absence> Absences { get; set; } = null!;
    public DbSet<Availability> Availabilities { get; set; } = null!;
    public DbSet<TimeSlot> TimeSlots { get; set; } = null!;
    public DbSet<WeekSchedule> WeekSchedules { get; set; } = null!;
    public DbSet<DaySchedule> DaySchedules { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Konfiguracja tabeli User
        modelBuilder.Entity<User>()
            .HasMany(u => u.Reservations)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Absences)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Availabilities)
            .WithOne(av => av.User)
            .HasForeignKey(av => av.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Konfiguracja tabeli Reservation
        modelBuilder.Entity<Reservation>()
            .Property(r => r.Type)
            .IsRequired()
            .HasMaxLength(50);

        modelBuilder.Entity<Reservation>()
            .Property(r => r.Gender)
            .IsRequired()
            .HasMaxLength(10);

        // Konfiguracja tabeli Availability
        modelBuilder.Entity<Availability>()
            .Property(a => a.Type)
            .IsRequired()
            .HasMaxLength(20);

        modelBuilder.Entity<Availability>()
            .Property(a => a.TimeRanges)
            .HasColumnType("json");

        modelBuilder.Entity<Availability>()
            .Property(a => a.DaysOfWeek)
            .HasColumnType("json");

        // Konfiguracja tabeli TimeSlot
        modelBuilder.Entity<TimeSlot>()
            .HasOne(ts => ts.Reservation)
            .WithMany()
            .HasForeignKey(ts => ts.ReservationId)
            .OnDelete(DeleteBehavior.SetNull);

        // Konfiguracja tabeli WeekSchedule
        modelBuilder.Entity<WeekSchedule>()
            .HasMany(ws => ws.Days)
            .WithOne(ds => ds.WeekSchedule)
            .HasForeignKey(ds => ds.WeekScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Konfiguracja tabeli DaySchedule
        modelBuilder.Entity<DaySchedule>()
            .HasMany(ds => ds.Slots)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
