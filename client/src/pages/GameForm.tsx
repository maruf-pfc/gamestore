import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { fetchGenres, fetchGame, createGame, updateGame, type Genre } from '../services/api';

export default function GameForm() {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const isEditing = Boolean(id);

    const [genres, setGenres] = useState<Genre[]>([]);
    const [loading, setLoading] = useState(isEditing);
    
    const [formData, setFormData] = useState({
        name: '',
        genreId: '',
        price: '',
        releaseDate: ''
    });

    useEffect(() => {
        const loadInitialData = async () => {
            try {
                const genresData = await fetchGenres();
                setGenres(genresData);
                
                if (genresData.length > 0) {
                    setFormData(prev => ({ ...prev, genreId: String(genresData[0].id) }));
                }

                if (isEditing && id) {
                    const gameData = await fetchGame(Number(id));
                    // Map string genre mapping to a genreId based on the options (or update API to return genreId).
                    // The API `fetchGame` returns `Game` with a string `genre`. We have to reverse-map it.
                    const matchedGenre = genresData.find(g => g.name === gameData.genre);
                    setFormData({
                        name: gameData.name,
                        genreId: matchedGenre ? String(matchedGenre.id) : String(genresData[0].id),
                        price: String(gameData.price),
                        releaseDate: gameData.releaseDate
                    });
                }
            } catch (error) {
                console.error(error);
            } finally {
                setLoading(false);
            }
        };

        loadInitialData();
    }, [id, isEditing]);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        
        try {
            const parsedData = {
                name: formData.name,
                genreId: Number(formData.genreId),
                price: parseFloat(formData.price),
                releaseDate: formData.releaseDate
            };
            
            if (isEditing && id) {
                await updateGame(Number(id), parsedData);
            } else {
                await createGame(parsedData);
            }
            navigate('/');
        } catch (error) {
            console.error('Submission failed', error);
            alert('Failed to save the game. Please check your inputs.');
        }
    };

    if (loading) return (
        <div className="page-container center-content">
            <div className="spinner"></div>
            <p className="loading-text">Loading...</p>
        </div>
    );

    return (
        <div className="page-container page-container-form">
            <div className="form-wrapper glass-card">
                <header className="page-header">
                    <h2>{isEditing ? 'Edit Game' : 'Add New Game'}</h2>
                    <p>{isEditing ? 'Update the details for this title.' : 'Expand the catalog with a brand new title.'}</p>
                </header>

                <form onSubmit={handleSubmit} className="game-form">
                    <div className="form-group">
                        <label htmlFor="name">Game Name</label>
                        <input 
                            type="text" 
                            id="name" 
                            name="name" 
                            value={formData.name} 
                            onChange={handleChange} 
                            required 
                            placeholder="e.g. Elden Ring"
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="genreId">Genre</label>
                        <div className="select-wrapper">
                            <select 
                                id="genreId" 
                                name="genreId" 
                                value={formData.genreId} 
                                onChange={handleChange} 
                                required
                            >
                                {genres.map(g => (
                                    <option key={g.id} value={g.id}>{g.name}</option>
                                ))}
                            </select>
                        </div>
                    </div>

                    <div className="form-row">
                        <div className="form-group flex-1">
                            <label htmlFor="price">Price ($)</label>
                            <input 
                                type="number" 
                                id="price" 
                                name="price" 
                                step="0.01" 
                                min="1" 
                                value={formData.price} 
                                onChange={handleChange} 
                                required 
                                placeholder="59.99"
                            />
                        </div>

                        <div className="form-group flex-1">
                            <label htmlFor="releaseDate">Release Date</label>
                            <input 
                                type="date" 
                                id="releaseDate" 
                                name="releaseDate" 
                                value={formData.releaseDate} 
                                onChange={handleChange} 
                                required 
                            />
                        </div>
                    </div>

                    <div className="form-actions">
                        <button type="button" className="btn btn-outline" onClick={() => navigate('/')}>Cancel</button>
                        <button type="submit" className="btn btn-primary">{isEditing ? 'Save Changes' : 'Create Game'}</button>
                    </div>
                </form>
            </div>
        </div>
    );
}
