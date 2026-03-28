import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { fetchGames, deleteGame, type Game } from '../services/api';

export default function GameList() {
    const [games, setGames] = useState<Game[]>([]);
    const [loading, setLoading] = useState(true);

    const loadGames = async () => {
        try {
            const data = await fetchGames();
            setGames(data);
        } catch (error) {
            console.error(error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadGames();
    }, []);

    const handleDelete = async (id: number) => {
        if (confirm('Are you sure you want to delete this game limit?')) {
            try {
                await deleteGame(id);
                setGames(games.filter((g) => g.id !== id));
            } catch (error) {
                console.error(error);
            }
        }
    };

    if (loading) return (
        <div className="page-container center-content">
            <div className="spinner"></div>
            <p className="loading-text">Loading catalog...</p>
        </div>
    );

    return (
        <div className="page-container">
            <header className="page-header">
                <h2>Game Catalog</h2>
                <p>Manage your collection of premium titles.</p>
            </header>
            
            {games.length === 0 ? (
                <div className="empty-state glass-card">
                    <div className="empty-icon">📂</div>
                    <h3>No games found</h3>
                    <p>Get started by adding your first premium title to the collection.</p>
                    <Link to="/edit" className="btn btn-primary mt-4">Add a Game</Link>
                </div>
            ) : (
                <div className="grid">
                    {games.map((game) => (
                        <div key={game.id} className="card glass-card">
                            <div className="card-header">
                                <span className="genre-badge">{game.genre}</span>
                                <h3 className="game-title">{game.name}</h3>
                            </div>
                            <div className="card-body">
                                <p className="price">${game.price.toFixed(2)}</p>
                                <p className="release-date">Released: {new Date(game.releaseDate).toLocaleDateString()}</p>
                            </div>
                            <div className="card-footer">
                                <Link to={`/edit/${game.id}`} className="btn btn-outline">Edit</Link>
                                <button onClick={() => handleDelete(game.id)} className="btn btn-danger">Delete</button>
                            </div>
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
}
