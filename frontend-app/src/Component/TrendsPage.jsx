import React, { useEffect, useState } from "react";
import axios from "axios";

const TrendsPage = () => {
  const [blogs, setBlogs] = useState([]);
  const [loading, setLoading] = useState(true);

  if (loading) {
    return <div>Loading...</div>;
  }

  return (
    <div>
      <h1>Trending Developer Blogs</h1>
      <div className="blogs-container">
        {blogs.map((blog, index) => (
          <div key={index} className="blog-card">
            <h2>{blog.title}</h2>
            <p>by {blog.author}</p>
            <p>{blog.summary}</p>
            <a href={blog.url} target="_blank" rel="noopener noreferrer">
              Read more
            </a>
          </div>
        ))}
      </div>
    </div>
  );
};

export default TrendsPage;
