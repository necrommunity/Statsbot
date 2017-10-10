import sys
sys.path.append('..')
import toofz, steam, category


index = category.indexer()

while True:
	print('q?')
	aa = input()
	if aa == 'exit':
		sys.exit()
	lb = index.get_certain_board(aa)
	print(steam.fetch_lb(lb))
