import steam

characters = ['All Chars DLC', 'All Chars', 'Story', 'Ensemble',
				'Aria', 'Bard', 'Bolt', 'Cadence', 'Chaunter', 'Coda',
				'Diamond', 'Dorian', 'Dove', 'Eli', 'Klarinetta',
				'Mary', 'Melody', 'Monk', 'Nocturna', 'Reaper', 'Suzu', 'Tempo']
run_type = ['Speed', 'Score', 'Deathless']
extras = ['No Return', 'Hard', 'Phasing', 'Mystery', 'Randomizer', 'Low%']
version = ['Amplified', 'Classic']


def caseless_in(check, source):
	if type(check) is str:
		return check.casefold() in source.casefold()
	for item in check:
		if item.casefold() in source.casefold():
			return True
	return False


def pad(q, i):
	if 'DLC' in q:
		q = 'All (13)'
	elif 'All' in q:
		q = 'All (9)'
	if i == 0:
		i = len(q)
	q += ' ' * (i - len(q))
	return q


def score_string(s, m): #ty jakk <3
	if m.run == 'Speed':
		ms = 100000000 - int(s)
		h, ms = divmod(ms, 60*60*1000)
		m, ms = divmod(ms, 60*1000)
		s, ms = divmod(ms, 1000)
		ms = round(ms / 10.0)

		if ms == 100:
			ms = 99

		result = ''

		if h:
			result += '%d:'%(h)
		else:
			result += '  '

		result += '%02d:'%(m)
		result += '%02d.%02d'%(s, ms)
		return result

	if m.run == 'Deathless':
		result = ''
		if s < 1000:
			result += ' '*4
		else:
			result += ' '*(7-len(str(s)))

		wins, s = divmod(s, 100)
		zone, s = divmod(s, 10)
		
		if m.char != 'Aria':
			return result + '{} ({}-{})'.format(wins, zone + 1, s + 1)
		if m.ver == 'Amplified':
			return result + '{} ({}-{})'.format(wins, 5- zone, s + 1)
		return result + '{} ({}-{})'.format(wins, 4- zone, s + 1)

	else:
		s = str(s)
		return ' ' * (6-len(s)) + s


def rank_string(rank):
	s = ' '*(6-len(str(rank))) + str(rank)
	if rank % 100 / 10 == 1:
		return s + 'th'
	rank %= 10
	if rank == 1:
		return s + 'st'
	elif rank == 2:
		return s + 'nd'
	elif rank == 3:
		return s + 'rd'
	else:
		return s + 'th'


def extract_char(q):
	for c in characters:
		if c == 'All Chars DLC':
			if caseless_in([c, 'DLC', '13'], q):
				return c
		if c == 'All Chars':
			if caseless_in([c, 'All', '9'], q):
				return c
		if caseless_in(c, q):
			return c
	return 'Cadence'


def extract_run(q,):
	for r in run_type:
		if caseless_in(r, q):
			return r
	return 'Speed'


def extract_extra(q):
	for e in extras:
		if e == 'No Return':
			if caseless_in(['Return', 'NR'], q):
				return e
		elif e == 'Randomizer':
			if caseless_in('Rando', q):
				return e
		elif caseless_in(e, q):
			return e
		elif e == "Low%":
			if caseless_in('low', q):
				return e
	return 'Standard'


def extract_ver(q, default):
	for v in version:
		if caseless_in(v, q):
			return v
	return default


def extract_seeded(q):
	if caseless_in('Seeded', q):
		return True
	return False


class indexer:
	def __init__(self):
		self.dict = steam.parse_index()


	def get_matching_boards(self, q, run=None):
		if not run:
			run = extract_run(q)
		extra = extract_extra(q)
		ver = extract_ver(q, 'Amplified')
		seeded = extract_seeded(q)
		matches = []
		for v in self.dict.values():
			if v.eq_no_char(run, extra, ver, seeded):
				matches.append(v)
		return matches


	def get_certain_board(self, q):
		char = extract_char(q)
		run = extract_run(q)
		extra = extract_extra(q)
		ver = extract_ver(q, 'Amplified')
		seeded = extract_seeded(q)
		for v in self.dict.values():
			if v.eq_no_id(char, run, extra, ver, seeded):
				return v


class leaderboard:
	def __init__(self, name, default, lbid=0):

		self.char = extract_char(name)
		self.run = extract_run(name)
		self.extra = extract_extra(name)
		self.ver = extract_ver(name, default)
		self.seeded = extract_seeded(name)

		self.lbid = lbid


	def eq_no_char(self, run, extra, ver, seeded):
		if self.run == run and self.extra == extra and self.ver == ver and self.seeded == seeded:
			return True
		return False
	
	def eq_no_id(self, char, run, extra, ver, seeded):
		if self.char == char and self.run == run and self.extra == extra and self.ver == ver and self.seeded == seeded:
			return True
		return False

	def __eq__(self, other):
		return self.__dict__ == other.__dict__

	def __str__(self):
		return str(self.__dict__)

	def __repr__(self):
		t = self.char, self.run, self.extra, self.ver, self.seeded
		return str(t)

