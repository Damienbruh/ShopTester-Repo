import { useEffect, useState } from "react";
import { useParams } from "react-router";

export default function Details() {
    const [data, setData] = useState({});
    const { id } = useParams();

    useEffect(() => {
        console.log("Product ID:", id);
        fetchProduct(id);
    }, [id]);

    const fetchProduct = async (id) => {
        const response = await fetch(`/api/products/${id}`);
        if (response.ok) {
            const data = await response.json();
            setData(data);
            console.log("Product Data:", data);
        }
    };


    return (
        <div>
            <h1>Product Details</h1>
            <p>Product ID: {data.id}</p>
            <p>Product Name: {data.name}</p>
            <p>Product Category: {data.category}</p>
            <img style={{ width: "50%" }} src={data.image_url} alt={data.name} />
            <p>Product Price: ${data.price}</p>
            <p>Product Description: {data.description}</p>
        </div>
    );
}