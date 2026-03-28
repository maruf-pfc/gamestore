import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Navigation from './components/Navigation';
import GameList from './pages/GameList';
import GameForm from './pages/GameForm';

function App() {
  return (
    <Router>
      <Navigation />
      <main className="main-content">
        <Routes>
          <Route path="/" element={<GameList />} />
          <Route path="/edit" element={<GameForm />} />
          <Route path="/edit/:id" element={<GameForm />} />
        </Routes>
      </main>
    </Router>
  );
}

export default App;
