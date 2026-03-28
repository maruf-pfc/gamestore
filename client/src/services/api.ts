export const API_URL = 'http://localhost:5270';

export interface Game {
    id: number;
    name: string;
    genre: string;
    price: number;
    releaseDate: string;
}

export interface CreateGameDto {
    name: string;
    genreId: number;
    price: number;
    releaseDate: string;
}

export interface UpdateGameDto {
    name: string;
    genreId: number;
    price: number;
    releaseDate: string;
}

export interface Genre {
    id: number;
    name: string;
}

export const fetchGames = async (): Promise<Game[]> => {
    const res = await fetch(`${API_URL}/games`);
    if (!res.ok) throw new Error('Failed to fetch games');
    return res.json();
};

export const fetchGame = async (id: number): Promise<Game> => {
    const res = await fetch(`${API_URL}/games/${id}`);
    if (!res.ok) throw new Error('Failed to fetch game');
    return res.json();
};

export const createGame = async (game: CreateGameDto): Promise<Game> => {
    const res = await fetch(`${API_URL}/games`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(game),
    });
    if (!res.ok) throw new Error('Failed to create game');
    return res.json();
};

export const updateGame = async (id: number, game: UpdateGameDto): Promise<void> => {
    const res = await fetch(`${API_URL}/games/${id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(game),
    });
    if (!res.ok) throw new Error('Failed to update game');
};

export const deleteGame = async (id: number): Promise<void> => {
    const res = await fetch(`${API_URL}/games/${id}`, {
        method: 'DELETE',
    });
    if (!res.ok) throw new Error('Failed to delete game');
};

export const fetchGenres = async (): Promise<Genre[]> => {
    const res = await fetch(`${API_URL}/genres`);
    if (!res.ok) throw new Error('Failed to fetch genres');
    return res.json();
};
