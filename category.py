
import json as jsonlib
import typing



id_characters = ['All Chars DLC', 'All Chars AMPSYNC', 'All Chars', 'Story', 'Ensemble',
				'Aria', 'Bard', 'Bolt', 'Cadence', 'Chaunter', 'Coda',
				'Diamond', 'Dorian', 'DOVE', 'Eli', 'Klarinetta',
				'Mary', 'Melody', 'Monk', 'Nocturna', 'Reaper', 'Suzu', 'Tempo',
				'Goldman', 'Coldsteel']
char_names = typing.Literal['All Chars DLC', 'All Chars', 'Story', 'Ensemble',
				'Aria', 'Bard', 'Bolt', 'Cadence', 'Chaunter', 'Coda',
				'Diamond', 'Dorian', 'Dove', 'Eli', 'Klarinetta',
				'Mary', 'Melody', 'Monk', 'Nocturna', 'Reaper', 'Suzu', 'Tempo',
				'Shovel Knight', 'Miku', 'Co-op']
chars_human = typing.get_args(char_names)
mode_strs = ['LOW', 'NO_BEAT', 'CUSTOM_MUSIC', 'RANDOMIZER', 'MYSTERY',
             'PHASING', 'HARD', 'NO_RETURN', 'DOUBLE_TEMPO']
co_op_strs = {'CO_OP8':8, 'CO_OP4':4, 'CO_OP3':3, 'CO_OP':2}
		
		
class Leaderboard:
	def __init__(self, lbid:str):
		self.char = None
		self.ranking = 'score'
		self.dlc = False
		self.sync = False
		self.seeded = False
		self.modes = []
		self.players = 1

		self.lbid = lbid
		self.from_lbid(lbid)

	def from_lbid(self, lbid):
		self.lbid = lbid
		self.dlc = 'DLC' in lbid
		self.sync = 'SYNC' in lbid
		self.seeded = 'SEEDED' in lbid 
		lbid = lbid.replace('HARDCORE', '')
		for c in id_characters:
			if c.replace(' ', '_') in lbid:
				self.char = c
				if c == 'DOVE':
					self.char = 'Dove'
				elif c == 'Goldman':
					self.char = 'Shovel Knight'
				elif c == 'Coldsteel':
					self.char = 'Miku'
				break
		if 'SPEEDRUN' in lbid:
			self.ranking = 'speed'
		elif 'DEATHLESS' in lbid:
			self.ranking = 'deathless'
		elif 'DUPING' in lbid:
			self.ranking = 'score duping'
		for m in mode_strs:
			if m in lbid:
				self.modes.append(m)
		for s, num in co_op_strs.items():
			if s in lbid:
				self.players = num
				if not self.char:
					self.char = 'Co-op'
				break
		if not self.char:
			self.char = 'Cadence'
 
	def __eq__(self, other):
		return self.__dict__ == other.__dict__

	def __str__(self):
		return str(self.__dict__)

	def __repr__(self):
		return str(self.__dict__)


def parse_lbid_list(input_file, outfile_file):
	with open(input_file) as f:
		lbids = f.read().split()
	lbs = []
	for lbid in lbids:
		if lbid.endswith('PROD'):
			obj = Leaderboard(lbid)
			lbs.append(obj)
	with open(outfile_file, 'w+') as f:
		jsonlib.dump(lbs, f, default=vars)


def score_string(s, lb): #ty jakk <3
	if lb.ranking == 'speed':
		ms = 100000000 - int(s)
		h, ms = divmod(ms, 60*60*1000)
		min, ms = divmod(ms, 60*1000)
		s, ms = divmod(ms, 1000)
		ms = round(ms / 10.0)

		if ms == 100:
			ms = 99

		result = ''

		if h:
			result += '%d:'%(h)
		else:
			result += '  '

		result += '%02d:'%(min)
		result += '%02d.%02d'%(s, ms)
		return result

	if lb.ranking == 'deathless':
		result = ''
		if s < 1000:
			result += ' '*4
		else:
			result += ' '*(7-len(str(s)))

		wins, s = divmod(s, 100)
		zone, s = divmod(s, 10)
  
		
		
		if lb.char != 'Aria':
			if zone == 0:
				return result + f'{wins} (win)'
			return result + '{} ({}-{})'.format(wins, zone + 1, s + 1)
		if lb.dlc:
			if zone == 9:
				return result + f'{wins} (win)'
			return result + '{} ({}-{})'.format(wins, 5-zone, s + 1)
		return result + '{} ({}-{})'.format(wins, 4-zone, s + 1)

	else:
		s = str(s)
		return s.rjust(6)
# parse_lbid_list('../leaderboards.txt', 'leaderboards.json')




