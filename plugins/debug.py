import sys
sys.path.append('..')
import toofz

while True:
	print('q?')
	aa = input()
	if aa == 'exit':
		sys.exit()
	#print(toofz.entries('naymin', 'score', aa))
	#print(toofz.list(['wil', 'revalize', 'goof', 'baba', 'lilac', 'cheesy', 'staekk', 'grimy', 'yamir', 'tufwfo', 'mac', 'axem', 'abu', 'fubz', 'thedarkfreack'], 'speed'))
	print(toofz.score(aa))