
import re
import collections

input_file_name = "pride_and_prejudice.txt"
all_words = []
word_freqs = collections.defaultdict(int)
unique_words = []
freq_freqs = collections.defaultdict(int)


          
def main():
    with open(input_file_name, "r") as file:
        for line in file:
            line = line.strip()
            if not line:
                continue
            line_words = line.split(' ')
        # for each word on each line, append it to all_words
        # once all words is fully created and populated, iterate through it:
        #     change to lowercase 
        #     remove all non alphabetic characters
        #     append cleaned words to all_words_clean
        # Iterate over all_words_clean
        #     write each word (including duplicates) on its on line to file allwords.txt
        #     use defualtdict to count frequency of every word
        # Iterate over dict
        #     if d[word] = 1, append it to list unique_words
        # Iterate over unique words, qriting each word to file uniquewords.txt
        # Iterate over dict again, writing each  value and its corresponding count on each line to file wordfrequency.txt
            for word in line_words:
                
                print(f"Raw Word: {word}")
                word = word.lower()
                word_clean = re.findall(r"[a-zA-Z]", word)
                word_clean = "".join(word_clean)
                print(f"Clean Word: {word_clean}")
                if word_clean:
                    all_words.append(word_clean)
                    word_freqs[word_clean] += 1
    for word in word_freqs.keys():
        freq_freqs[word_freqs[word]] += 1
        if word_freqs[word] == 1:
            unique_words.append(word)
        
    
    
    

    print(f"Total words: {len(all_words)}")
    print(f"Unique words: {len(unique_words)}")


    with open("allwords.txt", "w") as output_file:
        for word in all_words:
            output_file.write(word + '\n')
    with open("uniquewords.txt", "w") as output_file:
        for word in unique_words:
            output_file.write(word + "\n")
    
    with open("wordfrequency.txt", "w") as output_file:
        for count in sorted(freq_freqs.keys()):
            output_file.write(f"{count}: {freq_freqs[count]}\n")
    
    


if __name__ == "__main__":
    main()

    
    
    
