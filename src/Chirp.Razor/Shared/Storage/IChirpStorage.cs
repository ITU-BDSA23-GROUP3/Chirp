using Chirp.Razor.Storage.Types;

namespace Chirp.Razor.Storage;

public interface IChirpStorage
{
    public void StoreCheep(Cheep entity);
    
    public void StoreCheeps(List<Cheep> entities);
    
    public List<Cheep> GetCheepsFromAuthor(int pageNumber, string author);
    
    public IEnumerable<Cheep> GetCheeps();

    public IEnumerable<Cheep> GetCheepsPerPage(int pageNumber, int amount);

    public int Count();
}
