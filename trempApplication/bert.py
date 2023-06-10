from sentence_transformers import SentenceTransformer
from sklearn.metrics.pairwise import cosine_similarity
import sys

def string_similarity(string1, string2):
    # Load pre-trained model
    model = SentenceTransformer('bert-base-nli-mean-tokens')
    
    # Encode the input strings
    embeddings = model.encode([string1, string2], convert_to_tensor=True)
    
    # Calculate cosine similarity
    similarity = cosine_similarity(embeddings[0].unsqueeze(0), embeddings[1].unsqueeze(0))
    
    return similarity[0][0]

if __name__ == '__main__':
    if len(sys.argv) < 3:
        sys.exit(1)
    
    string1 = sys.argv[1]
    string2 = sys.argv[2]
    
    similarity = string_similarity(string1, string2)
    print(similarity)


