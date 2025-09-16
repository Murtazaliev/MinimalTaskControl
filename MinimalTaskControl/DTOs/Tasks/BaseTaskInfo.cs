using MinimalTaskControl.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace MinimalTaskControl.WebApi.DTOs.Tasks
{
    public abstract class BaseTaskInfo
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = default!;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [StringLength(100)]
        public string Author { get; set; } = default!;

        [StringLength(100)]
        public string? Assignee { get; set; }

        public TasksPriority Priority { get; set; } = TasksPriority.Medium;

        public Guid? ParentTaskId { get; set; }
    }
}
