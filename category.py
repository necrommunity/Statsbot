
characters = ['dlc', 'all', 'story', 'aria', 'bard', 'bolt',
				'coda', 'dorian', 'dove', 'eli', 'melody', 'monk',
				'nocturna', 'diamond', 'tempo', 'mary']
run_type = ['speed', 'score', 'deathless']
extras = ['return', 'hard', 'phasing', 'mystery', 'rando']


def parse(q):
	category = ['cadence', 'speed', 'amplified', 'standard', '']

	for c in characters:
		if c in q:
			category[0] = c
			break

	for r in run_type:
		if r in q:
			category[1] = r
			break

	if 'classic' in q:
		category[2] = 'classic'

	for e in extras:
		if e in q:
			category[3] = e
			break

	if 'seeded' in q:
		category[4] = 'seeded'

	return category

def pad(q, i, isString):
	if isString:
		q = q.capitalize()
		if 'mode' in q:
			q = 'Story'
		if 'amplified' in q:
			q = 'DLC'
		elif 'All' in q:
			q = 'All'
	if i == 0:
		i = len(q)
	q += ' ' * (i - len(q))
	return q
