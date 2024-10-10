const url = "https://localhost:44383/api"

export async function GetAllRepositories() {
    const response = await fetch(`${url}/repository`);

    if (!response.ok) {
        throw new Error('Failed to fetch repositories');
    }

    const data = await response.json();
    return data;
}