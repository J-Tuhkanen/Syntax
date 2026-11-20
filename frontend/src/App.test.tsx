import { render, screen } from '@testing-library/react';
import App from 'App';

afterEach(() => {
  vi.unstubAllGlobals();
});

test('shows the login link when there is no session', async () => {
  // No session cookie: GET /api/authentication/session answers 204.
  vi.stubGlobal('fetch', vi.fn().mockResolvedValue(new Response(null, { status: 204 })));

  render(<App />);

  expect(await screen.findByRole('link', { name: 'Login' })).toBeInTheDocument();
});
