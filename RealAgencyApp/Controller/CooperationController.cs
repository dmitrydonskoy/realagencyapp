using Microsoft.AspNetCore.Mvc;
using RealAgencyModels.BusinessLogic;
using RealAgencyModels.DTO;

namespace RealAgencyApp.Controller
{
	[ApiController]
	[Route("api/[controller]")]
	public class CooperationController : ControllerBase
	{
		private readonly CooperationService _cooperationService;
        private readonly UserService _userService;
        public CooperationController(CooperationService cooperationService, UserService userService)
		{
			_cooperationService = cooperationService;
			_userService = userService;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<CooperationDTO>> GetById(int id)
		{
			var cooperation = await _cooperationService.GetByIdAsync(id);
			if (cooperation == null) return NotFound();
			return Ok(cooperation);
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<CooperationDTO>>> GetAll()
		{
			var cooperations = await _cooperationService.GetAllAsync();
			return Ok(cooperations);
		}

		[HttpPost]
		public async Task<ActionResult<CooperationDTO>> Create(CooperationDTO dto)
		{
			var createdCooperation = await _cooperationService.CreateAsync(dto);
			return CreatedAtAction(nameof(GetById), new { id = createdCooperation.Id }, createdCooperation);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, CooperationDTO dto)
		{
			if (id != dto.Id) return BadRequest("ID mismatch");

			var updatedCooperation = await _cooperationService.UpdateAsync(id, dto);
			if (updatedCooperation == null) return NotFound();

			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var isDeleted = await _cooperationService.DeleteAsync(id);
			if (!isDeleted) return NotFound();

			return NoContent();
		}

        [HttpGet("/api/Cooperation/agent/{bidPartnerId}")]
        public async Task<IActionResult> GetCooperationsByAgent(int bidPartnerId)
        {
            var cooperations = await _cooperationService.GetCooperationsByAgentAsync(bidPartnerId);
            foreach (var cooperation in cooperations)
            {
                var client = await _userService.GetByIdAsync(cooperation.Biduserid);
                cooperation.ClientName = client?.Name ?? "Unknown";
            }
            return Ok(cooperations);
        }

        [HttpPost("api/Cooperation/SendProposal")]
        public async Task<IActionResult> SendProposal([FromBody] ProposalRequest proposal)
        {
            if (proposal == null || proposal.AnnouncementId == 0 || proposal.ClientId == 0)
            {
                return BadRequest("Invalid proposal data.");
            }

            await _cooperationService.SendProposalAsync(proposal.AnnouncementId, proposal.ClientId);
            return Ok();
        }

        [HttpGet("api/Cooperation/user/{userId}")]
        public async Task<IActionResult> GetCooperationByUserId(int userId)
        {
            var cooperation = await _cooperationService.GetCooperationByUserIdAsync(userId);

         

            return Ok(cooperation);
        }

        [HttpGet("api/Cooperation/{cooperationId}/Announcements")]
        public async Task<IActionResult> GetAnnouncementsByCooperation(int cooperationId)
        {
            var announcements = await _cooperationService.GetAnnouncementsByCooperationIdAsync(cooperationId);

           

            return Ok(announcements);
        }
    }
}
