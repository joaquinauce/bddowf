namespace WebAPIAFA.Models.Dtos.ClubSponsorsDto
{
    public class ClubSponsorPatchDto
    {
        public int IdClubSponsors { get; set; }

        public int IdClub { get; set; }
        public int IdSponsor { get; set; }
    }
}
