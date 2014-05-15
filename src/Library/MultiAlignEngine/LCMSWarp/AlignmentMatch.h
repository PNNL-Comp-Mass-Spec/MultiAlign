#pragma once
namespace MultiAlignEngine
{
	namespace Alignment
	{
		class AlignmentMatch
		{
			public:
				double	mdouble_net_start; 
				double	mdouble_net_end; 
				int		mint_section_start; 
				int		mint_section_end;

				double	mdouble_net_start_2; 
				double	mdouble_net_end_2; 
				int		mint_section_start_2; 
				int		mint_section_end_2;

				// score of the alignments between the two maps till (and including) this section match. 
				double	mdouble_alignment_score; 
				// score of just the match between the two maps and their sections. 
				double	mdouble_match_score; 

				void Set(double net_start_a, double net_end_a, int section_start_a, int section_end_a, 
						double net_start_b, double net_end_b, int section_start_b, int section_end_b, 
						double alignment_score, double match_score)
				{
					mdouble_net_start = net_start_a; 
					mdouble_net_end = net_end_a; 
					mdouble_net_start_2 = net_start_b; 
					mdouble_net_end_2 = net_end_b; 

					mint_section_start = section_start_a; 
					mint_section_end = section_end_a; 
					mint_section_start_2 = section_start_b; 
					mint_section_end_2 = section_end_b; 

					mdouble_alignment_score = alignment_score; 
					mdouble_match_score = match_score; 
				}

				AlignmentMatch(void);
				~AlignmentMatch(void);
		};
	}
}