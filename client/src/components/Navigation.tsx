import { Link } from 'react-router-dom';

export default function Navigation() {
    return (
        <nav className="glass-nav">
            <div className="nav-container">
                <Link to="/" className="nav-brand">
                    <span className="logo-icon">🎮</span> <span className="logo-text">GameStore</span>
                </Link>
                <div className="nav-links">
                    <Link to="/" className="nav-link">Catalog</Link>
                    <Link to="/edit" className="btn btn-primary">Add New Game</Link>
                </div>
            </div>
        </nav>
    );
}
